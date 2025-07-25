"use strict";
let selectedUser = null;

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

document.addEventListener("DOMContentLoaded", function () {
    const chatLinks = document.querySelectorAll(".chat-link");
    const receiverInput = document.getElementById("receiverInput");
    const selectedReceiverDisplay = document.getElementById("selectedReceiverDisplay");
    const sendButton = document.getElementById("sendButton");
    const messageInput = document.getElementById("messageInput");
    const fileInput = document.getElementById("fileInput");
    const messagesList = document.getElementById("messagesList");
        chatLinks.forEach(link => {
            link.addEventListener("click", async function (e) {
                const currentUser = document.getElementById("currentUsername").value;
                e.preventDefault();
                selectedUser = link.dataset.username;
                receiverInput.value = selectedUser;
                selectedReceiverDisplay.textContent = selectedUser;                            

                fetch(`/api/messages/${currentUser}/${selectedUser}`)
                    .then(response => response.json())
                    .then(messages => {
                        messagesList.innerHTML = "";
                        messages.forEach(msg => {
                            const li = document.createElement("li");
                            li.classList.add("message");
                            li.classList.add(msg.senderUsername == currentUser ? "message-right" : "message-left");

                            li.innerHTML = `
                                <div class="message-sender">From ${msg.senderUsername} To ${msg.receiverUsername}:</div>
                                <div>${msg.content}</div>
                                <div class= "message-time">${new Date(msg.sentAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</div>`;
                            messagesList.appendChild(li);
                        });
                        const container = document.querySelector(".messages-list-container");
                        if (container) {
                            container.scrollTop = container.scrollHeight;
                        }
                })
                toggleSendButton();            
            });
        }); 
function toggleSendButton() {
    const hasReceiver = receiverInput.value.trim() !== "";
    const hasMessageOrFile = messageInput.value.trim() !== "" || fileInput.files.length > 0;
    sendButton.disabled = !(hasReceiver && hasMessageOrFile);
}
        messageInput.addEventListener("input", toggleSendButton);
        fileInput.addEventListener("change", toggleSendButton);

        sendButton.addEventListener("click", async () => {
            const sender = document.getElementById("currentUsername").value;
            const receiver = receiverInput.value;
            const message = messageInput.value.trim();
            let file = fileInput.files[0];
            let fileUrl = "";

            if (!receiver) {
                alert("Please select a user to send message.");
                return;
            }
            if (!message && !file) {
                alert("Please enter a message or select an image.");
                return;
            }
            if (file) {
                const formData = new FormData();
                formData.append("file", file);
                try {
                    const response = await fetch("/api/uploads", {
                        method: "POST",
                        body: formData
                    });
                    if (response.ok) {
                        const data = await response.json();
                        fileUrl = data.fileUrl;
                    } else {
                        alert("File upload failed.");
                        return;
                    }
                } catch {
                    alert("File upload error.");
                    return;
                }
            }

            const baseUrl = window.location.origin;
            const fullFileUrl = fileUrl && !fileUrl.startsWith('http') ? `${baseUrl}${fileUrl}` : fileUrl;
            const finalMessage = fullFileUrl
                ? `<div style="text-align: right; margin-bottom: 8px;">
         <div style="display:inline-block; max-width:140px; padding:4px 8px; border:1px solid #ccc; border-radius:4px; background:#f1f1f1; font-size:13px; word-break:break-word;">
             📎 <a href="${fullFileUrl}" target="_blank" style="text-decoration:none; color:#007bff;" download>File</a>
         </div>
       </div>
       <div>${message}</div>`
                : message;


            try {
                await connection.invoke("SendPrivateMessageToUser", receiver, finalMessage);
            } catch (err) {
                console.error(err.toString());
            }

            messageInput.value = "";
            fileInput.value = "";
            toggleSendButton();
        });


        messageInput.addEventListener("keypress", e => {
            if (e.key === "Enter" && !e.shiftKey) {
                e.preventDefault();
                if (!sendButton.disabled) {
                    sendButton.click();
                }
            }
        });

        connection.on("ReceiveMessage", (user, message, sentAt, receiverUsername) => {
            const currentUser = document.getElementById("currentUsername").value;

            if (!((user === currentUser && receiverUsername === selectedUser)||(user === selectedUser && receiverUsername === currentUser))) {
                return;
            }
            const isMe = user === currentUser;

            const li = document.createElement("li");
            li.classList.add("message");
            li.classList.add(isMe ? "message-right" : "message-left");

            li.innerHTML =
                `<div class="message-sender">From ${user} To ${receiverUsername}:</div>
        <div>${message}</div>
        <div class="message-time">${sentAt}</div>`;

            const messagesList = document.getElementById("messagesList");
            messagesList.appendChild(li);
            li.scrollIntoView({ behavior: "smooth", block: "end" });
        });

        connection.start()
            .then(() => 
                console.log("Connected to SignalR hub."))                
            .catch(err => console.error(err.toString()));

        toggleSendButton();
});