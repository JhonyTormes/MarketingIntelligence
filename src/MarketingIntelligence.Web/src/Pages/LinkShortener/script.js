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
                <td>${linkItem.inputOriginalUrl}</td>
                <td><a href="#" style="color: #0056b3; font-weight: bold;">${linkItem.shortUrl}</a></td>
                <td>${linkItem.clicks}</td>
            </tr>
        `;
        tableBody.innerHTML += newRow;
    });
}

form.addEventListener('submit', function(event) {
    event.preventDefault();

    const randomCode = Math.random().toString(36).substring(2, 7);
    const fakeShortUrl = 'mi.link/' + randomCode;

    const newLinkData = {
        campaign: inputCampaignName.value,
        inputOriginalUrl: inputOriginalUrl.value,
        shortUrl: fakeShortUrl,
        clicks: 0 // Começa com zero cliques
    };

    myLinks.unshift(newLinkData);

    renderLinks();

    form.reset();
});