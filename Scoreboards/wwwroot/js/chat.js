"use strict";


var connection = new signalR.HubConnectionBuilder().withUrl("/scoreboardsHub").build();

//On getting "ReceiveMessage" tag updates <ul> with new data (name, message)
connection.on("Notify", function (message) {
    //reload the page
    location.reload();
    //document.getElementById("liveMessageBar").innerHTML = message;
});

//Starts connection
connection.start().catch(function (err) {
    return console.error(err.toString());
});

