redirectToCheckout = function (sessionId) {
    var stripe = Stripe('stripe key');
    stripe.redirectToCheckout({
        sessionId: sessionId
    });
};