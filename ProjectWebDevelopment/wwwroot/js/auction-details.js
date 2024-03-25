import { AuctionCountdown } from "./elements/auction-countdown.js";
import { AuctionBid } from "./elements/auction-bid.js";
import './signalr/dist/browser/signalr.js';

const auctionId = window.auctionId;
const auctionBidsList = document.getElementById("auctionBids");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/auctionhub")
    .build();

connection.on("ReceiveBidUpdate", function (message) {
    // Handle bid update, e.g., display a notification
    const jsonData = JSON.parse(message);

    const auctionBid = document.createElement('auction-bid');
    auctionBid.setAttribute('name', jsonData.Name);
    auctionBid.setAttribute('price', jsonData.Price);
    
    const listItem = document.createElement('li');
    listItem.append(auctionBid);
    
    auctionBidsList.insertBefore(listItem, auctionBidsList.firstChild);

    // Update the minimum bid
    const nextMinimumElement = document.getElementById("minimimBid");
    const nextMinimum = jsonData.NextMinimum;

    if (nextMinimumElement) {
        nextMinimumElement.innerHTML = nextMinimum;
    }
});

connection.start()
    .then(function () {
        console.log("SignalR connected.");
        joinAuctionGroup(auctionId);
    })
    .catch(function (err) {
        console.error(err.toString());
    });

// Join auction group when necessary, e.g., when entering auction details page
function joinAuctionGroup(auctionId) {
    connection.invoke("JoinAuctionGroup", auctionId)
        .catch(function (err) {
            console.error(err.toString());
        });
}

// Leave auction group when necessary, e.g., when leaving auction details page
function leaveAuctionGroup(auctionId) {
    connection.invoke("LeaveAuctionGroup", auctionId)
        .catch(function (err) {
            console.error(err.toString());
        });
}