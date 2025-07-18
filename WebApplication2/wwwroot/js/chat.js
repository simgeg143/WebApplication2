"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

const userInput = document.getElementById("userInput");
const messageInput = document.getElementById("messageInput");
const sendButton = document.getElementById("sendButton");


function validateInputs() {
    const user = userInput.value.trim();
    const message = messageInput.value.trim();

    if (user && message) {
        sendButton.disabled = false;        
    } else {
        sendButton.disabled = true;
    }
}

messageInput.addEventListener("input", validateInputs);

window.addEventListener("DOMContentLoaded", validateInputs);

connection.start().then(function () {
    console.log("SignalR connection is successfull.");
    validateInputs();
}).catch(function (err) {
    console.error("Connection error:", err.toString());
});

sendButton.addEventListener("click", function (event) {
    const user = userInput.value.trim();
    const message = messageInput.value.trim();


    connection.invoke("SendMessage", message).catch(function (err) {
    });

    messageInput.value = "";
    validateInputs();
    event.preventDefault();
});
messageInput.addEventListener("keypress", function (event) {
    if (event.key === "Enter" && !event.shiftKey) {
        event.preventDefault();
        const sendButton = document.getElementById("sendButton");
        if (!sendButton.disabled) {
            sendButton.click();
        }
    }
});

connection.on("ReceiveMessage", function (user, message, sentAt) {
    const currentUser = document.getElementById("userInput").value;

    const messageContainer = document.createElement("div");
    messageContainer.classList.add("message");

    if (user === currentUser) {
        messageContainer.classList.add("message-right");
    } else {
        messageContainer.classList.add("message-left");
    }
    messageContainer.innerHTML = `<strong>${user}</strong>: ${message} <div class="message-time">${sentAt}</div>`;
    document.getElementById("messagesList").appendChild(messageContainer);

    const messagesList = document.getElementById("messagesList");
    messagesList.scrollTop = messagesList.scrollHeight;
});
