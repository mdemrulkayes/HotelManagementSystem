using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlazorInputFile;
using Business.Core;
using Business.DataModels;
using DataAccess.Data;
using HotelManagementSystem.BlazorServer.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace HotelManagementSystem.BlazorServer.Pages.HotelRoom
{
    public class CreateEditBase: ComponentBase
    {
        [Parameter]
        public int? Id { get; set; }
        internal HotelRoomRequestDTO HotelRoomModel { get; set; } = new HotelRoomRequestDTO();
        internal HotelRoomDTO HotelRoomDetails { get; set; } = new HotelRoomDTO();
        private HotelRoomImage RoomImage { get; set; } = new HotelRoomImage();
        private List<string> DeletedImageNames { get; set; } = new List<string>();

        internal string Title { get; set; } = "Create";
        internal bool IsProcessingStart { get; set; } = false;
        internal bool IsImageUploaded { get; set; } = false;
        internal bool IsImageUploadProcessStart { get; set; } = false;
        internal string ImageProcessMessage { get; set; } = string.Empty;

        [Inject]
        public IHotelRepository HotelRepository { get; set; }
        [Inject]
        public IHotelImagesRepository HotelImagesRepository { get; set; }
        [Inject]
        public IMapper Mapper { get; set; }
        [Inject]
        public IFileUpload FileUpload { get; set; }
        [Inject]
        internal IAuthenticationService AuthService { get; set; }
        [Inject]
        internal NavigationManager NavigationManager { get; set; }
        [Inject]
        internal IJSRuntime JSRuntime { get; set; }
        [Inject]
        internal IConfiguration Configuration { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (!await AuthService.IsUserAuthorized())
            {
                NavigationManager.NavigateTo("login", true);
                return;
            }
            if (Id != null)
            {
                Title = "Update";
                HotelRoomDetails = await HotelRepository.GetHotelRoom(Id.Value);
            }
            else
            {
                HotelRoomDetails = new HotelRoomDTO();
            }

            Mapper.Map(HotelRoomDetails, HotelRoomModel);
            if (HotelRoomDetails?.HotelRoomImages != null)
            {
                IsImageUploaded = true;
                HotelRoomModel.ImageUrls = HotelRoomDetails.HotelRoomImages.Select(x => x.RoomImageUrl).ToList();
            }
           
        }


        internal async Task HandleHotelRoomCreate()
        {
            try
            {
                IsProcessingStart = true;
                if (HotelRoomModel.Id != 0 && Title == "Update") 
                {
                    var roomDetailsByName = await HotelRepository.IsSameNameRoomAlreadyExists(HotelRoomModel.Name);
                    if (roomDetailsByName != null && roomDetailsByName.Id != HotelRoomModel.Id)
                    {
                        throw new Exception("Hotel Room name is already exists.");
                    }
                    //Update the hotel room here
                    var updateRoomResult = await HotelRepository.UpdateHotelRoom(HotelRoomModel.Id, HotelRoomModel);

                    if (HotelRoomModel.ImageUrls != null && HotelRoomModel.ImageUrls.Any())
                    {
                        //Delete hotel images from folder
                        if (DeletedImageNames != null && DeletedImageNames.Any())
                        {
                            foreach (var deletedImageName in DeletedImageNames)
                            {
                                var result = FileUpload.DeleteFile(deletedImageName);
                            }
                        }

                        //Delete Hotel Old Images
                        await HotelImagesRepository.DeleteHotelImageByHotelRoomId(updateRoomResult.Id);

                        //Create Hotel new Images
                        await AddHotelRoomImage(updateRoomResult);
                    }
                    
                    await JSRuntime.InvokeVoidAsync("ShowToaster", "success", "Success", "Hotel Room updated successfully");
                }
                else
                {
                    if (HotelRoomModel.ImageUrls == null || HotelRoomModel.ImageUrls.Count <= 0)
                    {
                        throw new Exception("Please upload image.");
                    }

                    var roomDetailsByName = await HotelRepository.IsSameNameRoomAlreadyExists(HotelRoomModel.Name);
                    if (roomDetailsByName != null)
                    {
                        throw new Exception("Hotel Room name is already exists.");
                    }

                    HotelRoomModel.UserId = AuthService.User.Id;
                    //Create new Hotel room here
                    var createdResult = await HotelRepository.CreateHotelRoom(HotelRoomModel);
                    //Create Hotel Images
                    await AddHotelRoomImage(createdResult);
                    HotelRoomModel = new HotelRoomRequestDTO();
                    HotelRoomDetails = new HotelRoomDTO();
                    await JSRuntime.InvokeVoidAsync("ShowToaster", "success", "Success", "Hotel Room created successfully.");
                }
                NavigationManager.NavigateTo("/");
                IsProcessingStart = false;
            }
            catch (Exception e)
            {
                IsProcessingStart = false;
                await JSRuntime.InvokeVoidAsync("ShowToaster", "error", "Error Occured", e.Message);
            }
        }

        public async Task HandleImageUpload(IFileListEntry[] files)
        {
            IsImageUploadProcessStart = true;
            ImageProcessMessage = "Please wait.. Images are uploading...";
            try
            {
                var images = new List<string>();
                if (files.Any())
                {
                    foreach (var file in files)
                    {
                        FileInfo fileInfo = new FileInfo(file.Name);
                        if (fileInfo.Extension.ToLower() == ".jpg" || fileInfo.Extension.ToLower() == ".png" || fileInfo.Extension.ToLower() == ".jpeg")
                        {
                            var uploadedImagePath = await FileUpload.UploadFile(file);
                            images.Add(uploadedImagePath);
                        }
                    }

                    if (images.Any())
                    {
                        if (HotelRoomModel.ImageUrls != null && HotelRoomModel.ImageUrls.Any())
                        {
                            HotelRoomModel.ImageUrls.AddRange(images);
                        }
                        else
                        {
                            HotelRoomModel.ImageUrls = new List<string>();
                            HotelRoomModel.ImageUrls.AddRange(images);
                        }
                        
                        IsImageUploaded = true;
                    }
                    else
                    {
                        await JSRuntime.InvokeVoidAsync("ShowToaster", "error", "Error Occured", "Image uploading failed");
                    }
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("ShowToaster", "error", "Error Occured", "Please select .jpg/.jpeg/.png file only");
                }
            }
            catch (Exception e)
            {
                await JSRuntime.InvokeVoidAsync("ShowToaster", "error", "Error Occured", e.Message);
            }

            IsImageUploadProcessStart = false;
            ImageProcessMessage = "";
        }

        private async Task AddHotelRoomImage(HotelRoomDTO roomDetails)
        {
            foreach (var imageUrl in HotelRoomModel.ImageUrls)
            {
                RoomImage = new HotelRoomImage()
                {
                    RoomId = roomDetails.Id,
                    RoomImageUrl = imageUrl
                };
                var createHotelRoomImage = await HotelImagesRepository.CreateHotelRoomImage(RoomImage);
            }
        }

        internal async Task DeletePhoto(string imageUrl)
        {
            IsImageUploadProcessStart = true;
            ImageProcessMessage = "Please wait.. Images are deleting...";
            try
            {
                var imageIndex = HotelRoomModel.ImageUrls.FindIndex(x => x == imageUrl);
                var imageName = imageUrl.Replace($"{Configuration["ImageUrl"]}/RoomImages/", "");
                if (HotelRoomModel.Id == 0 && Title == "Create")
                {
                    var result = FileUpload.DeleteFile(imageName);
                }
                else
                {
                    DeletedImageNames ??= new List<string>();
                    DeletedImageNames.Add(imageName);
                }
                
                HotelRoomModel.ImageUrls.RemoveAt(imageIndex);
            }
            catch (Exception e)
            {
                await JSRuntime.InvokeVoidAsync("ShowToaster", "error", "Error Occured", e.Message);
            }
            IsImageUploadProcessStart = false;
            ImageProcessMessage = "";
            IsImageUploaded = HotelRoomModel.ImageUrls.Any();
            StateHasChanged();
        }
    }
}
