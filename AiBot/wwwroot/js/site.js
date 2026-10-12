const messages = document.getElementById("messages");
const input = document.getElementById("question");
const button = document.getElementById("send");

messages.scrollTop = messages.scrollHeight;

function addMessage(who, text) {
    const p = document.createElement("p");
    const name = document.createElement("strong");
    name.textContent = who + ": ";
    const body = document.createElement("span");
    body.className = "msg-text";
    body.textContent = text;
    p.appendChild(name);
    p.appendChild(body);
    messages.appendChild(p);
    messages.scrollTop = messages.scrollHeight;
    return body;
}

async function send() {
    const question = input.value.trim();
    if (!question) return;

    addMessage("You", question);
    input.value = "";
    button.disabled = true;
    const reply = addMessage("Bot", "Thinking...");

    try {
        const response = await fetch("/Chat/Ask", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ question: question })
        });

        if (response.ok) {
            const data = await response.json();
            reply.textContent = data.answer;
        } else {
            reply.textContent = "Something went wrong (" + response.status + ").";
        }
    } catch {
        reply.textContent = "Could not reach the server.";
    }

    messages.scrollTop = messages.scrollHeight;
    button.disabled = false;
    input.focus();
}

button.addEventListener("click", send);
input.addEventListener("keydown", function (e) {
    if (e.key === "Enter") send();
});