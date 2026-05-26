document.getElementById('loginForm').addEventListener('submit', async function(event) {

    event.preventDefault();

    const formData = new FormData(this);
    const requestData = Object.fromEntries(formData.entries());

    try {
        const response = await fetch('https://localhost:7118/api/identity/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(requestData)
        });

        if (response.ok) {
            const data = await response.json();
            
            localStorage.setItem('jwtToken', data.token);

            alert('Login efetuado com sucesso!');
            
            //Todo: Redirect the user to the logged-in area
            // window.location.href = '/dashboard.html'; 
        } else {
            const errorData = await response.json();
            
            alert(errorData.error || 'E-mail ou senha inválidos.');
        }
    } catch (error) {
        console.error('Erro de rede:', error);
        alert('Não foi possível conectar ao servidor. A API está rodando?');
    }
});