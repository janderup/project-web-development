import { AuctionCountdown } from "./elements/auction-countdown.js";
import { AuctionBid } from "./elements/auction-bid.js";
import './signalr/dist/browser/signalr.js';

const auctionDetails = (() => {
    const auctionId = window.auctionId;
    const auctionBidsList = document.getElementById("auctionBids");

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/auctionhub")
        .build();

    // When receiving an update from the server about a new bid
    connection.on("ReceiveBidUpdate", function (message) {
        // Handle bid update, e.g., display a notification
        const jsonData = JSON.parse(message);

        updateBidsList(jsonData.Name, jsonData.Price);

        const nextMinimum = jsonData.NextMinimum;
        updateMinimumBid(nextMinimum);
    });

    // Inserts a new bid to the list of bids and removes the last one
    const updateBidsList = (newBidName, newBidPrice) => {
        // Insert new bid
        const auctionBid = document.createElement('auction-bid');
        auctionBid.setAttribute('name', newBidName);
        auctionBid.setAttribute('price', newBidPrice);

        const listItem = document.createElement('li');
        listItem.append(auctionBid);

        auctionBidsList.insertBefore(listItem, auctionBidsList.firstChild);

        // Remove last child from list
        const lastChild = auctionBidsList.lastElementChild;
        if (lastChild) {
            auctionBidsList.removeChild(lastChild);
        }
    }

    // Updates the minimum bid in both the input field and the text underneath
    const updateMinimumBid = (nextMinimum) => {
        // Update the minimum bid
        const nextMinimumElement = document.getElementById("minimimBid");

        if (nextMinimumElement) {
            nextMinimumElement.innerHTML = nextMinimum;
        }

        // Update the minimum bid in the input
        const minimumBidInput = document.getElementById("minimumBidInput");

        if (minimumBidInput) {
            minimumBidInput.setAttribute("min", nextMinimum);
        }
    }

    // Starts the connection with Signal R
    const startConnection = () => {
        connection.start()
            .then(function () {
                joinAuctionGroup(auctionId);
            })
            .catch(function (err) {
                console.error(err.toString());
            });
    }

    // Join auction group when necessary, e.g., when entering auction details page
    const joinAuctionGroup = (auctionId) => {
        connection.invoke("JoinAuctionGroup", auctionId)
            .catch(function (err) {
                console.error(err.toString());
            });
    }

    // Leave auction group when necessary, e.g., when leaving auction details page
    const leaveAuctionGroup = (auctionId) => {
        connection.invoke("LeaveAuctionGroup", auctionId)
            .catch(function (err) {
                console.error(err.toString());
            });
    }

    return {
        startConnection,
        joinAuctionGroup,
        leaveAuctionGroup
    }
})();
auctionDetails.startConnection();