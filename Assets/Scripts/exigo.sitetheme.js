// Default themes per section
var sectionthemes = {
    "none": "default",
    "home": "default",
    "messages": "default",
    "events": "default",
    "organization": "default",
    "gameplan": "default",
    "commissions": "default",
    "ranks": "default",
    "orders": "default",
    "autoships": "default",
    "subscriptions": "default",
    "myaccount": "default"
};

if (!page.activenavigation) page.activenavigation = "none";

// Set the page theme
if (page.theme) $('body').addClass('theme-' + page.theme);
else $('body').addClass('theme-' + sectionthemes[page.activenavigation]);

// Set the active navigation
if (page.activenavigation) $('#sitenavigation li[data-key="' + page.activenavigation + '"]').addClass('active');