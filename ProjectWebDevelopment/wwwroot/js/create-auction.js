import { AuctionImagePreview } from "./elements/auction-image-preview.js";

const createAuctionPage = (function () {

    $('input[type="file"]').change(function (e) {
        $('#imagePreview').empty();
        var files = e.target.files;
        $.each(files, function (i, file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#imagePreview').append('<auction-image-preview src="' + e.target.result + '">');
            }
            reader.readAsDataURL(file);
        });
    });

    // Auction type
    const auctionTypeElement = document.getElementById('auctionType');
    const minimumBidInput = document.getElementById('MinimumBid');

    auctionTypeElement.addEventListener('change', () => {
        const selectedValue = auctionTypeElement.value;

        if (selectedValue == 'noMinimum') {
            minimumBidInput.value = '';
            minimumBidInput.setAttribute('disabled', 'true');
        } else {
            minimumBidInput.removeAttribute('disabled');
        }
    });

    return {};
})();