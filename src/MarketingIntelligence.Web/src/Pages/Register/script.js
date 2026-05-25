document.getElementById('registerForm').addEventListener('submit', async function(event) {
    event.preventDefault();

    const formData = new FormData(this);
    const requestData = Object.fromEntries(formData.entries());

    try {
        const response = await fetch('https://localhost:7118/api/identity/createUser', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(requestData)
        });

        if (response.ok) {
            alert('Conta criada com sucesso!');
            // TODO: Redirect the user to the login page.
        } else {
            const errorData = await response.json();
            console.error('Erro na API:', errorData);
            alert('Erro ao criar conta. Verifique o console para mais detalhes.');
        }
    } catch (error) {
        console.error('Erro de rede:', error);
        alert('Não foi possível conectar ao servidor. A API está rodando?');
    }
});
