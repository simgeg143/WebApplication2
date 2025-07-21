"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

document.getElementById("sendButton").disabled = true;

// Dropdown veya mesaj yazıldığında butonu aktif et
document.getElementById("receiverInput").addEventListener("change", toggleSendButton);
document.getElementById("messageInput").addEventListener("input", toggleSendButton);

function toggleSendButton() {
    const receiver = document.getElementById("receiverInput").value;
    const message = document.getElementById("messageInput").value.trim();

    document.getElementById("sendButton").disabled = (receiver === "" || message === "");
}

// Mesaj gönder
document.getElementById("sendButton").addEventListener("click", function (event) {
    const receiver = document.getElementById("receiverInput").value;
    const message = document.getElementById("messageInput").value;

    connection.invoke("SendMessageToUser", receiver, message).catch(function (err) {
        return console.error(err.toString());
    });

    document.getElementById("messageInput").value = "";
    toggleSendButton(); // mesaj kutusu boşalınca tekrar kontrol
    event.preventDefault();
});

// Gelen mesajı dinle
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
});

// Kullanıcı listesi güncellendiğinde dropdown'ı yenile
connection.on("UserUpdated", function (userList) {
    const currentUser = document.getElementById("userInput").value;
    const dropdown = document.getElementById("receiverInput");

    // Mevcut seçenekleri temizle
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
