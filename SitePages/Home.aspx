<%@ Page Language="C#" MasterPageFile="~/MasterPages/Corporate.Master" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script type="text/javascript" src="Assets/Scripts/jquery.min.js"></script>
    <script type="text/javascript" src="Assets/Scripts/jquery.fancybox.js"></script>
    <script type="text/javascript" src="Assets/Scripts/jquery.fancybox-thumbs.js"></script>


    <link href="Assets/Styles/jquery.fancybox.css" rel="stylesheet" />
    <link href="Assets/Styles/jquery.fancybox-thumbs.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="Assets/Styles/home.css" />


    <script type="text/javascript">
        $(document).ready(function () {
            /*
             *  Simple image gallery. Uses default settings
             */

            $('.fancybox').fancybox();

            /*
             *  Different effects
             */

            // Change title type, overlay closing speed
            $(".fancybox-effects-a").fancybox({
                helpers: {
                    title: {
                        type: 'outside'
                    },
                    overlay: {
                        speedOut: 0
                    }
                }
            });

            // Disable opening and closing animations, change title type
            $(".fancybox-effects-b").fancybox({
                openEffect: 'none',
                closeEffect: 'none',

                helpers: {
                    title: {
                        type: 'over'
                    }
                }
            });

            // Set custom style, close if clicked, change title type and overlay color
            $(".fancybox-effects-c").fancybox({
                wrapCSS: 'fancybox-custom',
                closeClick: true,

                openEffect: 'none',

                helpers: {
                    title: {
                        type: 'inside'
                    },
                    overlay: {
                        css: {
                            'background': 'rgba(238,238,238,0.85)'
                        }
                    }
                }
            });

            // Remove padding, set opening and closing animations, close if clicked and disable overlay
            $(".fancybox-effects-d").fancybox({
                padding: 0,

                openEffect: 'elastic',
                openSpeed: 150,

                closeEffect: 'elastic',
                closeSpeed: 150,

                closeClick: true,

                helpers: {
                    overlay: null
                }
            });

            /*
             *  Button helper. Disable animations, hide close button, change title type and content
             */

            $('.fancybox-buttons').fancybox({
                openEffect: 'none',
                closeEffect: 'none',

                prevEffect: 'none',
                nextEffect: 'none',

                closeBtn: false,

                helpers: {
                    title: {
                        type: 'inside'
                    },
                    buttons: {}
                },

                afterLoad: function () {
                    this.title = 'Image ' + (this.index + 1) + ' of ' + this.group.length + (this.title ? ' - ' + this.title : '');
                }
            });


            /*
             *  Thumbnail helper. Disable animations, hide close button, arrows and slide to next gallery item if clicked
             */

            $('.fancybox-thumbs').fancybox({
                prevEffect: 'none',
                nextEffect: 'none',

                closeBtn: false,
                arrows: false,
                nextClick: true,

                helpers: {
                    thumbs: {
                        width: 50,
                        height: 50
                    }
                }
            });

            /*
             *  Media helper. Group items, disable animations, hide arrows, enable media and button helpers.
            */
            $('.fancybox-media')
                .attr('rel', 'media-gallery')
                .fancybox({
                    openEffect: 'none',
                    closeEffect: 'none',
                    prevEffect: 'none',
                    nextEffect: 'none',

                    arrows: false,
                    helpers: {
                        media: {},
                        buttons: {}
                    }
                });

            /*
             *  Open manually
             */

            $("#fancybox-manual-a").click(function () {
                alert();
                $.fancybox.open('1_b.jpg');
            });

            $("#fancybox-manual-b").click(function () {
                alert();
                $.fancybox.open({
                    href: 'iframe.html',
                    type: 'iframe',
                    padding: 5
                });
            });

            $("#fancybox-manual-c").click(function () {
                $.fancybox.open([
                    {
                        href: '1_b.jpg',
                        title: 'My title'
                    }, {
                        href: '2_b.jpg',
                        title: '2nd title'
                    }, {
                        href: '3_b.jpg'
                    }
                ], {
                    helpers: {
                        thumbs: {
                            width: 75,
                            height: 50
                        }
                    }
                });
            });


        });
    </script>


</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="Content">
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            if ($('INPUT[type="hidden"][id*="ShowMessage"]').val() != '') {
                $(function () {
                    $('DIV#Message').children('p').hide();
                    $('DIV#Message DIV.Close').children('p').hide();
                    $('DIV#Message').show();
                    $('DIV#Message').animate({ width: '400px' }, 400, function () {
                        $('DIV#Message DIV.Close').children('p').fadeIn('fast');
                    });
                    $('DIV#Message').children('p').fadeIn('slow');
                    setTimeout(function () {
                        $('DIV#Message DIV.Close').children('p').fadeOut("fast", function () {
                            $('DIV#Message').children('p').fadeOut("fast");
                            $('DIV#Message').animate({ width: '0px' }, 400);
                            $('DIV#Message').fadeOut("fast");
                        });
                    }, 5000);
                });
            }

            $('DIV#Message DIV.Close').hover(function () {
                $(this).css('background-color', '#315885');
            }).click(function () {
                $(this).css('background-color', '#5aa1f3');
                $(this).delay(200).fadeOut("fast", function () {
                    $('DIV#Message').children('p').fadeOut("fast");
                    $('DIV#Message').animate({ width: '0px' }, 400);
                    $('DIV#Message').fadeOut("fast");
                });

            });
        });
    </script>
    <asp:HiddenField ID="ShowMessage" runat="server" />


    <div class="content-grey-round group">

        <div class="tour-content-right">
            <div class="tour-content">
                <h1 class="leftTitle">Request a Game Plan Report</h1>
                <div style="margin: auto; width: 275px;">

                    <a href="GamePlanSubmissionForm.aspx" class="fancybox fancybox.iframe transImageLink">
                        <img src="Assets/Images/gameplan.png" width="273" height="71"></a>


                    <div style="padding: 50px"></div>

                    <div id="DatePicker">
                        Date:
                        <input type="text" id="datepicker" />
                    </div>


                </div>
            </div>





        </div>

    </div>





</asp:Content>
