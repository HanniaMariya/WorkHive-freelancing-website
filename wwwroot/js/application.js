var connection = new signalR.HubConnectionBuilder().withUrl("/appHub").build();
/*document.getElementById("sendButton").disabled = true;*/

connection.on("ReceiveUpdate", function (appId, newStatus) {
    alert(`Application ${appId} status updated to ${newStatus}`);

    // Find the buttons using `data-id`
    let hireButton = document.querySelector(`button[data-id='${appId}'].btn-success`);
    let rejectButton = document.querySelector(`button[data-id='${appId}'].btn-danger`);

    if (newStatus === "Accepted") {
        if (hireButton)
            hireButton.textContent = "Accepted";
    }
    else if (newStatus === "Rejected") {
        if (rejectButton)
            rejectButton.textContent = "Rejected";
    }

    if (hireButton) hireButton.disabled = true;
    if (rejectButton) rejectButton.disabled = true;

});
async function startConnection() {
    try {
        await connection.start();
        console.log("Connected to SignalR hub.");

        document.querySelectorAll(".AppStatusBtn").forEach(button => {
            button.addEventListener("click", async function (event) {
                let appId = parseInt(this.getAttribute("data-id"));
                let newStatus = this.classList.contains("btn-success") ? "Accepted" : "Rejected";

                if (connection.state !== signalR.HubConnectionState.Connected) {
                    console.error("SignalR is not connected yet.");
                    return;
                }

                try {
                    await connection.invoke("UpdateApplicationStatus", appId, newStatus);
                } catch (err) {
                    console.error("SignalR invoke error:", err);
                }

                event.preventDefault();
            });
        });

    } catch (err) {
        console.error("Error connecting to SignalR:", err);
        setTimeout(startConnection, 5000); // Retry after 5 seconds
    }
}

startConnection();
