let myLinks = [];

const form = document.getElementById('create-link-form');
const tableBody = document.getElementById('links-table-body');
const inputOriginalUrl = document.querySelector('input[name="Original-Url"]');
const inputCampaignName = document.querySelector('input[name="Campaign-Name"]');

function renderLinks(){
    tableBody.innerHTML = '';

    myLinks.forEach(function(linkItem){
        const newRow = `
            <tr>
                <td>${linkItem.campaign}</td>
                <td>${linkItem.originalUrl}</td> 
                <td><a href="${linkItem.shortUrl}" target="_blank" style="color: #0056b3; font-weight: bold;">${linkItem.shortUrl}</a></td>
                <td>${linkItem.clicks}</td>
            </tr>
        `;
        tableBody.innerHTML += newRow;
    });
}

form.addEventListener('submit', async function(event) {
    event.preventDefault();

    const originalUrlValue = inputOriginalUrl.value;
    const campaignValue = inputCampaignName.value;

    const token = localStorage.getItem('jwtToken');
    if (!token) {
        alert("You must be logged in to shorten a link.");
        window.location.href = "../Login/index.html";
        return;
    }

    try {
        const response = await fetch('https://localhost:7118/api/links', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}` // Attach the JWT to identify the user
            },
            body: JSON.stringify({
                originalUrl: originalUrlValue,
                campaignName: campaignValue
            })
        });

        if (response.ok) {
            const data = await response.json();

            const realShortUrl = `https://localhost:7118/${data.shortCode}`;

            const newLinkData = {
                campaign: data.campaignName || campaignValue,
                originalUrl: data.originalUrl,
                shortUrl: realShortUrl,
                clicks: 0 
            };

            myLinks.unshift(newLinkData);
            renderLinks();
            
            form.reset();

        } else {
            const errorText = await response.text();
            alert(`API Error: ${errorText}`);
        }

    } catch (error) {
        console.error('Network Error:', error);
        alert('Could not connect to the C# API. Is the server running?');
    }
});

async function loadMyLinks() {
    try {
        const token = localStorage.getItem('jwtToken');

        if (!token) {
            window.location.href = "../Login/index.html";
            return; 
        }

        const response = await fetch('https://localhost:7118/api/links', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const data = await response.json();
            
            myLinks = data;
            
            renderLinks();
            
        } else if (response.status === 401) {
            alert("Sua sessão expirou. Por favor, faça login novamente.");
            localStorage.removeItem('jwtToken');
            window.location.href = "../Login/index.html";
        } else {
            console.error("Erro da API ao buscar links.");
        }
    } catch (error) {
        console.error("Erro de conexão. A API está rodando?", error);
    }
}

document.addEventListener("DOMContentLoaded", () => {
    loadMyLinks();
});