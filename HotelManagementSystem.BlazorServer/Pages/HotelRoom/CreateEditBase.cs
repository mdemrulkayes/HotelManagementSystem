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

        internal string Title { get; set; } = "Create";
        internal bool IsProcessingStart { get; set; } = false;
        internal string SuccessMessage { get; set; }
        internal string ErrorMessage { get; set; }
        internal bool FileUploadSuccessMessage { get; set; } = false;
        internal bool FileUploadErrorMessage { get; set; } = false;
        internal bool IsImageUploaded { get; set; } = false; 

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
                        //Delete Hotel Old Images
                        await HotelImagesRepository.DeleteHotelImageByHotelRoomId(updateRoomResult.Id);

                        //Create Hotel new Images
                        await AddHotelRoomImage(updateRoomResult);
                    }
                    
                    SuccessMessage = "Hotel Room updated successfully";
                    await JSRuntime.InvokeVoidAsync("ShowToaster", "success", "Success", SuccessMessage);
                }
                else
                {
                    if (HotelRoomModel.ImageUrls == null || HotelRoomModel.ImageUrls.Count <= 0)
                    {
                        FileUploadErrorMessage = true;
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
                    SuccessMessage = "Hotel Room created successfully.";
                    await JSRuntime.InvokeVoidAsync("ShowToaster", "success", "Success", SuccessMessage);
                }
                NavigationManager.NavigateTo("/");
                IsProcessingStart = false;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                IsProcessingStart = false;
                await JSRuntime.InvokeVoidAsync("ShowToaster", "error", "Error Occured", ErrorMessage);
            }

            
        }

        public async Task HandleImageUpload(IFileListEntry[] files)
        {
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
                            FileUploadSuccessMessage = true;
                        }
                        else
                        {
                            HotelRoomModel.ImageUrls = new List<string>();
                            HotelRoomModel.ImageUrls.AddRange(images);
                            FileUploadSuccessMessage = true;
                        }
                        
                        IsImageUploaded = true;
                    }
                    else
                    {
                        FileUploadErrorMessage = true;
                    }
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                FileUploadErrorMessage = true;
            }
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

        internal void DeletePhoto(string imageUrl)
        {
            var imageIndex = HotelRoomModel.ImageUrls.FindIndex(x => x == imageUrl);
            HotelRoomModel.ImageUrls.RemoveAt(imageIndex);
            StateHasChanged();
        }
    }
}
