/*
    * version 0.0.1
    * requires jQuery.js, bootstrap.js
    * 2015 Anton Permiakov
*/
(function ($) {
    $.fn.ellipsisTooltip = function (options) {

        if (this.length < 1) {
            return "No ellipsis found";
        }

        var $ellipsis = this;

        var params = $.extend({
            content: undefined,
            parent: $ellipsis.parent(),
            width: $ellipsis.parent().width(),
            resize: true,
            placement: "top",
            container: "body",
            template: '<div class="tooltip" role="tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner" style="max-width: 250px; word-wrap: break-word"></div></div>',
            ellipsis: "%%%"
        }, options);

        var calcBlockWidth = function () {
            var block = $('<div id="ellipsis-wrapper">');

            if (typeof (params.content) === "undefined") {
                block.append(params.ellipsis[0].outerHTML);
                $("body").append(block);
            } else {
                block.append(params.content);
                $("body").append(block);
                params.ellipsis = $("#ellipsis-wrapper").children();
            }

            return block.width();
        }

        var createEllipsis = function (tooltip) {
            if (tooltip) {
                styleEllipsis(tooltip);
                params.ellipsis
				  .attr("title", params.ellipsis.text())
				  .tooltip({
				      placement: params.placement,
				      container: params.container,
                      template: params.template
				  })
				  .attr({ "title": "", "data-toggle": "tooltip" });
            } else {
                styleEllipsis(tooltip);
                params.ellipsis.removeAttr("data-toggle");
                params.ellipsis.tooltip('destroy');
            }
            removeBlock();
        }

        var styleEllipsis = function (isEllipsis) {
            if (isEllipsis) {
                params.ellipsis.css({
                    'white-space': 'nowrap',
                    'overflow': 'hidden',
                    'text-overflow': 'ellipsis'
                });
            } else {
                params.ellipsis.removeAttr("style");
            }
        }

        var removeBlock = function () {
            $("#ellipsis-wrapper").remove();
        }

        var initEllipsis = function (resize) {
            if (calcBlockWidth() > params.width) {
                createEllipsis(true);
            } else {
                createEllipsis(false);
            }
            if (resize) {
                resizeMonitor();
            }
        }

        var resizeMonitor = function () {
            $(window).resize(function () {
                clearTimeout(resizeTimer);
                var resizeTimer = setTimeout(function () {
                    initEllipsis(false);
                }, 60);
            })
        }
        
        $ellipsis.each(function () {
            params.ellipsis = $(this);
            initEllipsis(params.resize);
        })

        return params.ellipsis[0].outerHTML;
    }
})(jQuery);
$(document).ready(function () {
    $("[data-ellipsis]").ellipsisTooltip();
})
