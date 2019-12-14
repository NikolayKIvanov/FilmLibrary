$('.input').focus(function () {
    if ($(this).parent().find(".text-danger").children().length !== 0) {
        $(this).parent().find(".line-box").find(".line").css('background-color', 'red');
    }
    $(this).parent().find(".label-txt").addClass('label-active');
    $(this).parent().find(".line-box").find(".line").addClass('line-style');
});

$(".input").focusout(function () {
    if ($(this).parent().find(".text-danger").children().length === 0) {
        if ($(this).val() === '') {
            $(this).parent().find(".label-txt").removeClass('label-active');
        };
    }
});

$(".text-danger").change(function () {
    if ($(this).find(".text-danger").children().length === 0) {
        $(this).parent.find(".line-box").find(".line").css('background-color', '#8BC34A');
    } else {
        $(this).parent().find(".line-box").find(".line").css('background-color', 'red');
    }
});
