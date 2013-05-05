<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UnexpectedError.aspx.cs" Inherits="UnexpectedError" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <meta charset="utf-8">
    <title></title>
    <style>
        html, body { font-family: Arial, sans-serif; background: url(Assets/Images/bgErrorPage.png) repeat top left; }
        .clear { clear: both; }
        .errorcode { position: absolute; bottom: 0; right: 0; font-size: 256px; font-weight: bold; color: #eee; }
        .contentwrapper { width: 630px; margin: 100px auto 0 auto; padding: 40px; border-radius: 8px; background-color: #EEE; border: 5px dashed #CCC; }

        h1 { color: #2c99b9; font-size: 45px; letter-spacing: -2px; margin: 0; padding: 0;  text-align: center; }
        h2 { color: #718593; letter-spacing: -1px; font-family: Georgia, serif; font-weight: normal; text-align: center; }
        a { color: #2cb957; text-decoration: none; }
        hr { height: 1px; background: #CCC; border: 0;  }
        p { color: #999; font-size: 13px; text-align: center; }

            p.options { text-align: center; margin-top: 50px; }
        a.button { font-size: 27px; letter-spacing: -1px; text-decoration: none; color: white; padding: 15px 30px 15px 60px; background-color: #2cb957; border-radius: 6px; }
        a.back { background: url(Assets/Images/icnErrorPageBack.png) no-repeat 20px 17px #2cb957; }
        a.home { background: url(Assets/Images/icnErrorPageHome.png) no-repeat 20px 17px #2c99b9; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="errorcode">500</div>
        <div class="contentwrapper">
        <h1>I'm sorry, but it appears I broke your page.</h1>
        <h2>Don't panic - I've already sent our geeks a head's up.</h2>

            <p class="options">
                <a href="javascript:history.go(-1)" class="button back">Go Back</a>
                <a href="Default.aspx" class="button home">My Dashboard</a>
                <div class="clear"></div>
            </p>
        </div>
    </form>
</body>
</html>
