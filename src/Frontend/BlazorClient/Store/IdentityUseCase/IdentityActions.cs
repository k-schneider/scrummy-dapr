namespace Scrummy.BlazorClient.Store.IdentityUseCase;

public record HydrateIdentityAction(string Sid, string Nickname);
public record RememberNicknameAction(string Nickname);
