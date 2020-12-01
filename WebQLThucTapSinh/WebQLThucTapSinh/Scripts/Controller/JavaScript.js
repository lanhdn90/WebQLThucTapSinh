$(document).ready(function () {
    $(window).scroll(function (event) {
        var pos_body = $('html,body').scrollTop();
        if (pos_body > 20) {
            $('.header-section').addClass('co-dinh-menu');
        }
        else {
            $('.header-section').removeClass('co-dinh-menu');
        }
    });
    $('.dangnhap3').click(function () {
        $('html,body').animate({ scrollTop: 0 }, 1400)

    });
});