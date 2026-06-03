const loginForm = document.getElementById('loginForm');

document.getElementById('loginForm').addEventListener('submit', async function(event) {

    event.preventDefault();

    const emailValue = document.getElementById('email').value;
    const passwordValue = document.getElementById('password').value;

    try {

        const response = await fetch('https://localhost:7118/api/identity/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ 
                email: emailValue, 
                password: passwordValue 
            })
        });

        if (response.ok) {
            const data = await response.json();
            
            localStorage.setItem('jwtToken', data.token);

            window.location.href = "../LinkShortener/index.html";
        } else {
            const errorData = await response.json();
            
            alert(errorData.error || 'E-mail ou senha inválidos.');
        }
    } catch (error) {
        console.error('Erro de rede:', error);
        alert('Não foi possível conectar ao servidor. A API está rodando?');
    }
});