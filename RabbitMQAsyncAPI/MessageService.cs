using System;

public class MessageService
{
    [Channel("user/signedup")]
    [Message("UsuarioRegistrado")]
    public void HandleUserSignup()
    {
        Console.WriteLine("Usuário registrado");
    }

    [Channel("user/loggedin")]
    [Message("UsuarioLogado")]
    public void HandleUserLogin()
    {
        Console.WriteLine("Usuário logado");
    }
}
