/*
** nopCommerce ajax cart implementation
*/


var AjaxCart = {
    loadWaiting: false,
    usepopupnotifications: false,
    topcartselector: '',
    topwishlistselector: '',
    flyoutcartselector: '',
    localized_data: false,

    init: function (usepopupnotifications, topcartselector, topwishlistselector, flyoutcartselector, localized_data) {
        this.loadWaiting = false;
        this.usepopupnotifications = usepopupnotifications;
        this.topcartselector = topcartselector;
        this.topwishlistselector = topwishlistselector;
        this.flyoutcartselector = flyoutcartselector;
        this.localized_data = localized_data;
    },

    setLoadWaiting: function (display) {
        displayAjaxLoading(display);
        this.loadWaiting = display;
    },

    //add a product to the cart/wishlist from the catalog pages
    addproducttocart_catalog: function (urladd) {
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);

        var postData = {};
        addAntiForgeryToken(postData);

        $.ajax({
            cache: false,
            url: urladd,
            type: "POST",
            data: postData,
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        });
    },

    //add a product to the cart/wishlist from the product details page
    addproducttocart_details: function (urladd, formselector) {
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);

        $.ajax({
            cache: false,
            url: urladd,
            data: $(formselector).serialize(),
            type: "POST",
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        });
    },

    //add a product to compare list
    addproducttocomparelist: function (urladd) {
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);

        var postData = {};
        addAntiForgeryToken(postData);

        $.ajax({
            cache: false,
            url: urladd,
            type: "POST",
            data: postData,
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        });
    },

    success_process: function (response) {
        if (response.updatetopcartsectionhtml) {
            $(AjaxCart.topcartselector).html(response.updatetopcartsectionhtml);
        }
        if (response.updatetopwishlistsectionhtml) {
            $(AjaxCart.topwishlistselector).html(response.updatetopwishlistsectionhtml);
        }
        if (response.updateflyoutcartsectionhtml) {
            $(AjaxCart.flyoutcartselector).replaceWith(response.updateflyoutcartsectionhtml);
        }
        if (response.message) {
            //display notification
            if (response.success === true) {
                //success
                if (AjaxCart.usepopupnotifications === true) {
                  displayBarNotificationSaljiDalje(response.message, 'success', true);
                }
                else {
                    //specify timeout for success messages
                  displayBarNotificationSaljiDalje(response.message, 'success', 3500);
                }
            }
            else {
                //error
                if (AjaxCart.usepopupnotifications === true) {
                  displayBarNotificationSaljiDalje(response.message, 'error', true);
                }
                else {
                    //no timeout for errors
                  displayBarNotificationSaljiDalje(response.message, 'error', 0);
                }
            }
            return false;
        }
        if (response.redirect) {
            location.href = response.redirect;
            return true;
        }
        return false;
    },

    resetLoadWaiting: function () {
        AjaxCart.setLoadWaiting(false);
    },

    ajaxFailure: function () {
        alert(this.localized_data.AjaxCartFailure);
    }
};

function displayBarNotificationSaljiDalje(message, messagetype, timeout) {
  var notificationTimeout;

  var messages = typeof message === 'string' ? [message] : message;
  if (messages.length === 0)
    return;

  //types: success, error, warning
  var cssclass = ['success', 'error', 'warning'].indexOf(messagetype) !== -1 ? messagetype : 'success';

  //remove previous CSS classes and notifications
  $('#bar-notification')
    .removeClass('success')
    .removeClass('error')
    .removeClass('warning');
  $('#bar-notification').addClass("show");
  $('.bar-notification').remove();

  //add new notifications
  var htmlcode = document.createElement('div');

  //IE11 Does not support miltiple parameters for the add() & remove() methods
  htmlcode.classList.add('bar-notification', cssclass);
  htmlcode.classList.add(cssclass);

  //add close button for notification
  var close = document.createElement('span');
  close.classList.add('close');
  close.setAttribute('title', document.getElementById('bar-notification').dataset.close);

  for (var i = 0; i < messages.length; i++) {
    var content = document.createElement('span');
    content.classList.add('content');
    content.innerHTML = messages[i];

    htmlcode.appendChild(content);
  }

  htmlcode.appendChild(close);

  $('#bar-notification')
    .append(htmlcode);

  $(htmlcode)
    .fadeIn('slow')
    .on('mouseenter', function() {
      clearTimeout(notificationTimeout);
    });

  //callback for notification removing
  var removeNoteItem = function () {
    $(htmlcode).remove();
  };

  $(close).on('click', function () {
    $(htmlcode).fadeOut('slow', removeNoteItem);
  });

  //timeout (if set)
  if (timeout > 0) {
    notificationTimeout = setTimeout(function () {
      $(htmlcode).fadeOut('slow', removeNoteItem);
    }, timeout);
  }
}