namespace eShop.Identity.API.Configuration;
/// <summary>
/// IdentityServer4 구성을 위한 설정 클래스입니다.
/// 이 클래스는 API 리소스, API 스코프, Identity 리소스 및 클라이언트를 정의합니다.
/// </summary>
/// <remarks>
/// - API 리소스: 시스템에서 보호해야 할 API 엔드포인트를 정의합니다.
/// - API 스코프: API에 대한 접근 권한의 범위를 정의합니다.
/// - Identity 리소스: 사용자 ID, 이름, 이메일 등 사용자 관련 데이터를 정의합니다.
/// - 클라이언트: API에 접근하고자 하는 애플리케이션을 정의합니다.
/// </remarks>

public class Config
{
    // ApiResources define the apis in your system
    /// <summary>
    /// 시스템에서 사용 가능한 API 리소스 목록을 반환합니다.
    /// </summary>
    /// <returns>
    /// 다음 API 리소스들을 포함하는 컬렉션:
    /// - orders: 주문 관련 서비스
    /// - basket: 장바구니 관련 서비스  
    /// - webhooks: 웹훅 등록 서비스
    /// </returns>
    public static IEnumerable<ApiResource> GetApis()
    {
        return new List<ApiResource>
        {
            new ApiResource("orders", "Orders Service"),
            new ApiResource("basket", "Basket Service"), 
            new ApiResource("webhooks", "Webhooks registration Service"),
        };
    }

    // ApiScope is used to protect the API 
    //The effect is the same as that of API resources in IdentityServer 3.x
    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>
        {
            new ApiScope("orders", "Orders Service"),
            new ApiScope("basket", "Basket Service"),
            new ApiScope("webhooks", "Webhooks registration Service"),
        };
    }

    // Identity resources are data like user ID, name, or email address of a user
    // see: http://docs.identityserver.io/en/release/configuration/resources.html
    public static IEnumerable<IdentityResource> GetResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };
    }

    // client want to access resources (aka scopes)
    public static IEnumerable<Client> GetClients(IConfiguration configuration)
    {
        return new List<Client>
        {
            new Client
            {
                ClientId = "maui",
                ClientName = "eShop MAUI OpenId Client",
                AllowedGrantTypes = GrantTypes.Code,                    
                //Used to retrieve the access token on the back channel.
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                RedirectUris = { configuration["MauiCallback"] },
                RequireConsent = false,
                RequirePkce = true,
                PostLogoutRedirectUris = { $"{configuration["MauiCallback"]}/Account/Redirecting" },
                //AllowedCorsOrigins = { "http://eshopxamarin" },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    "orders",
                    "basket",
                    "mobileshoppingagg",
                    "webhooks"
                },
                //Allow requesting refresh tokens for long lived API access
                AllowOfflineAccess = true,
                AllowAccessTokensViaBrowser = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                AccessTokenLifetime = 60*60*2, // 2 hours
                IdentityTokenLifetime= 60*60*2 // 2 hours
            },
            new Client
            {
                ClientId = "webapp",
                ClientName = "WebApp Client",
                ClientSecrets = new List<Secret>
                {
                    new Secret("secret".Sha256())
                },
                ClientUri = $"{configuration["WebAppClient"]}",                             // public uri of the client
                AllowedGrantTypes = GrantTypes.Code,
                AllowAccessTokensViaBrowser = false,
                RequireConsent = false,
                AllowOfflineAccess = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                RequirePkce = false,
                RedirectUris = new List<string>
                {
                    $"{configuration["WebAppClient"]}/signin-oidc"
                },
                PostLogoutRedirectUris = new List<string>
                {
                    $"{configuration["WebAppClient"]}/signout-callback-oidc"
                },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    "orders",
                    "basket",
                    "webshoppingagg",
                    "webhooks"
                },
                AccessTokenLifetime = 60*60*2, // 2 hours
                IdentityTokenLifetime= 60*60*2 // 2 hours
            },
            new Client
            {
                ClientId = "webhooksclient",
                ClientName = "Webhooks Client",
                ClientSecrets = new List<Secret>
                {
                    new Secret("secret".Sha256())
                },
                ClientUri = $"{configuration["WebhooksWebClient"]}",                             // public uri of the client
                AllowedGrantTypes = GrantTypes.Code,
                AllowAccessTokensViaBrowser = false,
                RequireConsent = false,
                AllowOfflineAccess = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                RedirectUris = new List<string>
                {
                    $"{configuration["WebhooksWebClient"]}/signin-oidc"
                },
                PostLogoutRedirectUris = new List<string>
                {
                    $"{configuration["WebhooksWebClient"]}/signout-callback-oidc"
                },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    "webhooks"
                },
                AccessTokenLifetime = 60*60*2, // 2 hours
                IdentityTokenLifetime= 60*60*2 // 2 hours
            },
            new Client
            {
                ClientId = "basketswaggerui",
                ClientName = "Basket Swagger UI",
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,

                RedirectUris = { $"{configuration["BasketApiClient"]}/swagger/oauth2-redirect.html" },
                PostLogoutRedirectUris = { $"{configuration["BasketApiClient"]}/swagger/" },

                AllowedScopes =
                {
                    "basket"
                }
            },
            new Client
            {
                ClientId = "orderingswaggerui",
                ClientName = "Ordering Swagger UI",
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,

                RedirectUris = { $"{configuration["OrderingApiClient"]}/swagger/oauth2-redirect.html" },
                PostLogoutRedirectUris = { $"{configuration["OrderingApiClient"]}/swagger/" },

                AllowedScopes =
                {
                    "orders"
                }
            },
            new Client
            {
                ClientId = "webhooksswaggerui",
                ClientName = "WebHooks Service Swagger UI",
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,

                RedirectUris = { $"{configuration["WebhooksApiClient"]}/swagger/oauth2-redirect.html" },
                PostLogoutRedirectUris = { $"{configuration["WebhooksApiClient"]}/swagger/" },

                AllowedScopes =
                {
                    "webhooks"
                }
            }
        };
    }
}
