using System.Net;

namespace GuildManager.Logic.API;

// Turns the exceptions an API call can raise into messages the player can
// understand. The texts are French on purpose: they are displayed in the
// game's UI (only the code and its comments are in English).
public static class OnlineErrors
{
    public static string Describe(Exception exception) => exception switch
    {
        ApiException { StatusCode: HttpStatusCode.Conflict } =>
            "Ce pseudo est déjà utilisé.",
        ApiException { StatusCode: HttpStatusCode.Unauthorized } =>
            "Pseudo ou mot de passe incorrect.",
        ApiException api =>
            $"Erreur du serveur : {api.Message}",
        HttpRequestException =>
            $"Serveur injoignable ({OnlineSession.BaseUrl}). Vérifie que l'API et la base de données sont lancées.",
        TaskCanceledException =>
            "Le serveur met trop de temps à répondre.",
        _ =>
            $"Erreur inattendue : {exception.Message}",
    };
}
