var rankadvancement = {};

rankadvancement.settings = {
    requesturl: '',
    wrapperselector: '#rankadvancement',
    ranknavigationselector: '#ranks',
    loadingimagetext: "<p style='text-align:center;'>We are generating your report. Please wait...<br /><br /><img src='Assets/Images/imgLoading.gif' alt='Loading...' title=Loading' /></p>",
    retrytimeout: null,
    retryinterval: 5000
};


rankadvancement.initializeSections = function (id) {
    rankadvancement.loadSection(rankadvancement.settings.wrapperselector, 'qualifications', id);
};
rankadvancement.loadSection = function (selector, dataKey, query) {
    $(selector).html(rankadvancement.settings.loadingimagetext);
    $.ajax({
        url: rankadvancement.settings.requesturl + '?datakey=' + dataKey + '&rankid=' + query,
        cache: false,
        method: 'GET',
        success: function (data) {
            $(selector).hide().html(data).fadeIn('fast');
            window.clearTimeout(rankadvancement.settings.loadingimagetext);
        },
        error: function (data, status, error) {
            $(selector).html(rankadvancement.settings.loadingimagetext);
            rankadvancement.settings.retrytimeout = window.setTimeout(function () {
                rankadvancement.initializeSections(query);
            }, rankadvancement.settings.retryinterval);
        }
    });
};
rankadvancement.init = function () {
    $(rankadvancement.settings.ranknavigationselector + ' a').on('click', function (event) {
        var $triggerElement = $(event.target);
        rankadvancement.initializeSections($triggerElement.attr('data-rank'));

        $(rankadvancement.settings.ranknavigationselector + ' li').removeClass('active');
        $triggerElement.parent('li').addClass('active');
    });
    $(rankadvancement.settings.ranknavigationselector + ' li.active a').triggerHandler('click');
};