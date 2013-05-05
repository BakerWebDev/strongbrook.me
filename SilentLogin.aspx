<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SilentLogin.aspx.cs" Inherits="SilentLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Assets/Scripts/jquery.min.js" type="text/javascript"></script>
    
</head>
<body>

<script>
    var isSafari = (/Safari/.test(navigator.userAgent));
    var firstTimeSession = 0;

    function submitSessionForm() {
        if (firstTimeSession == 0) {
            firstTimeSession = 1;
            $("#sessionform").submit();
            setTimeout(processApplication(), 2000);
        }
    }

    if (isSafari) {
        $("body").append('<iframe id="sessionframe" name="sessionframe" onload="submitSessionForm()" src="http://www.strongbrookbackoffice.com/public/silentlogin.aspx" style="display:none;"></iframe><form id="sessionform" enctype="application/x-www-form-urlencoded" action="http://www.strongbrookbackoffice.com/public/silentlogin.aspx" target="sessionframe" action="post"></form>');
    } else {
        processApplication();
    }

    function processApplication() {
    }
    </script>

</body>
</html>
