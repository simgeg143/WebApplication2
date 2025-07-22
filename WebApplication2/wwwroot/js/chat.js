"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

document.getElementById("sendButton").disabled = true;

document.getElementById("receiverInput").addEventListener("change", toggleSendButton);
document.getElementById("messageInput").addEventListener("input", toggleSendButton);
document.getElementById("imageInput").addEventListener("change", toggleSendButton);

function toggleSendButton() {
    const receiver = document.getElementById("receiverInput").value;
    const message = document.getElementById("messageInput").value.trim();
    const file = document.getElementById("imageInput").files[0]; 

    const hasMessageOrImage = message !== "" || file;
    document.getElementById("sendButton").disabled = (receiver === "" || !hasMessageOrImage);
}

document.getElementById("sendButton").addEventListener("click", async function (event) {
    event.preventDefault();

    const receiver = document.getElementById("receiverInput").value;
    const message = document.getElementById("messageInput").value.trim();

    const fileInput = document.getElementById("imageInput");
    const file = fileInput.files[0];
    let imageUrl = "";
    if (file) {
        const formData = new FormData();
        formData.append("file", file);

        const response = await fetch("/api/uploads", {
            method: "POST",
            body: formData
        });
        if (response.ok) {
            const data = await response.json();
            imageUrl = data.fileUrl;
        } else {
            alert("Image upload failed.");
            return;
        }
    }
    const baseUrl = window.location.origin;
    const fullImageUrl = imageUrl && !imageUrl.startsWith('http') ? `${baseUrl}${imageUrl}` : imageUrl;


    const finalMessage = fullImageUrl ? `${message}<br> <img src='${fullImageUrl}' style="max-width: 200px; height: auto">` : message;
    connection.invoke("SendPrivateMessageToUser",receiver, finalMessage).catch(function (err) {
        return console.error(err.toString());
    });

    document.getElementById("messageInput").value = "";
    fileInput.value = ""; // Clear the file input
    toggleSendButton(); 
});
document.getElementById("messageInput").addEventListener("keypress", function (event) {
    if (event.key === "Enter" && !event.shiftKey) {
        event.preventDefault();
        const sendButton = document.getElementById("sendButton");
        if (!sendButton.disabled) {
            sendButton.click();
        }
    }
});

connection.on("ReceivePrivateMessage", function (user, message, sentAt, receiverUsername) {
    const currentUser = document.getElementById("userInput").value;
    const isMe = user === currentUser;

    const li = document.createElement("li");
    li.classList.add("message");
    li.classList.add(isMe ? "message-right" : "message-left");

    li.innerHTML =
        `<div class="message-sender">From ${user} To ${receiverUsername}:</div>
        <div>${message}</div>
        <div class="message-time">${sentAt}</div>`;

    document.getElementById("messagesList").appendChild(li);

    const messagesList = document.getElementById("messagesList");
    messagesList.scrollTop = messagesList.scrollHeight; // Scroll to the bottom
});

connection.on("UserUpdated", function (userList) {
    const currentUser = document.getElementById("userInput").value;
    const dropdown = document.getElementById("receiverInput");

    dropdown.innerHTML = `<option value="">Select user</option>`;

    userList.forEach(function (user) {
        if (user.Username !== currentUser) {
            const option = document.createElement("option");
            option.value = user;
            option.textContent = user;
            dropdown.appendChild(option);
        }
    });
});

connection.start().then(function () {
    console.log("Connected to SignalR hub.");
    toggleSendButton();
}).catch(function (err) {
    return console.error(err.toString());
});
