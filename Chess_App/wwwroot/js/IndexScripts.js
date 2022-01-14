"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/IndexHub").build();

const elements = {
    PlayersList: document.querySelector("#ActivePlayerslist"),
    InvitePlace: document.querySelector("#inviteSpace"),
    GameHistory: document.querySelector("#GameHistory")
}


connection.start();

connection.on("ShowLoggedPlayers", function (name)
{
    let div = document.createElement('div');
    let span = document.createElement('span');
    let button = document.createElement('button');
    button.setAttribute('id', name);
    button.setAttribute('class', 'btn btn-outline-dark bg-dark text-white');   
    button.innerText = "Zaproś";
    button.addEventListener("click", function ()
    {
        connection.invoke("SendInvite", name);
    })
    span.innerText = name;
    div.setAttribute('id', name);
    div.append(span, button);
    elements.PlayersList.appendChild(div);
})



connection.on("ReceiveInvite", function (SenderName)
{
    let div = document.createElement('div');
    let span = document.createElement('span');
    let button = document.createElement('button');
    span.setAttribute('class', 'text-white');
    span.innerText = SenderName + ' Zaprasza do gry';
    button.innerText = "Zagraj";
    button.setAttribute('class', 'btn btn-outline-dark bg-dark text-white');
    button.addEventListener('click', function ()
    {
        connection.invoke("RedirectPlayers",SenderName);
    });
    div.append(span, button);
    elements.InvitePlace.appendChild(div);
})

connection.on("DisconnectPlayer", function (name)
{
    let div = document.getElementById(name);
    elements.PlayersList.removeChild(div);
})

connection.on("ClearList", function ()
{
    const node = document.getElementById("ActivePlayerslist");
    node.innerHTML = '';
})


connection.on("ShowPlayedGames", function (opponnent)
{
    let div = document.createElement('div');
    let span = document.createElement('span');
    let button = document.createElement('button');
    button.setAttribute('id', name);
    button.setAttribute('class', 'btn btn-outline-dark bg-dark text-white');
    button.innerText = "Sprawdź";
    button.addEventListener("click", function ()
    {
        alert("ok");
        window.location.href = '/Home/CheckGame';
    })
    span.innerText = "przeciwko " + opponnent;
    div.append(span, button);
    elements.GameHistory.appendChild(div);
})


connection.on("SendToRoom", function ()
{
    window.location.href = '/Home/Multi';
})



    
