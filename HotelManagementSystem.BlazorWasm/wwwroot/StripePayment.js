redirectToCheckout = function (sessionId) {
    var stripe = Stripe('pk_test_51HcC2NDg6YiCPiYXvu2iUetWULnsUMMQvZD4IzLyI2QEnD5YIXfgdqJntZKqFvpzCOxcSOcbeTnfdn7FT7kzakaE00CClfHRXs');
    stripe.redirectToCheckout({
        sessionId: sessionId
    });
};