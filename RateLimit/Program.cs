using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region Fixed Window
//Sabit bir zaman aral��� kullanarak istekleri s�n�rland�ran algoritmad�r.
//builder.Services.AddRateLimiter(options => {
//    options.AddFixedWindowLimiter("Basic", _options =>
//    {
//        _options.Window = TimeSpan.FromSeconds(12); //her 12 saniyede
//        _options.PermitLimit = 4; // 4 tane request g�nderme
//        _options.QueueLimit = 2; //4 tane fazla olursa 2 sini queue ya al daha fazlas�n� bo�lu�a al
//        _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; //process etmeye eskiden ba�la
//    });
//});
#endregion

#region Sliding Window
//Bulundu�u periodun yar�s�nda sonra 
//builder.Services.AddRateLimiter(options => {
//    options.AddSlidingWindowLimiter("Sliding",_options =>
//    {
//        _options.Window = TimeSpan.FromSeconds(12); //12 saniyede bir �al��mas� gerekti�i
//        _options.PermitLimit = 4; //her 12 saniyede 4 tane request i�lemesi
//        _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//        _options.QueueLimit = 2;
//        _options.SegmentsPerWindow = 2; //bir sonraki windowun en fazla 2 tanesini kullanabildi�ini g�stermek i�in her periot bir �nceki s�radan 2 tane harcama olas��� versin

//    });
//});
#endregion

#region Token Butcket
//Her bir periotta i�lecek request say�s� kadar token �retmektedir.
//E�er bu tokenlar kullan�ld�ysa di�er periottan bor� al�nabilir. Her periotta token �retim miktari kadar token �retilicek 
//bu �ekilde rate limit uygulacakt�r. Her periodun maximum token limit verilen sabir say� kadar olacakt�r

//builder.Services.AddRateLimiter(options => {
//    options.AddTokenBucketLimiter("Token", _options =>
//    {
//        _options.TokenLimit = 4; 
//        _options.TokensPerPeriod = 4;
//        _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//        _options.QueueLimit = 2;
//        _options.ReplenishmentPeriod = TimeSpan.FromSeconds(12);
//    });
//});
#endregion

#region Concurrency
//Asencron requestleri s�n�rland�rmak i�in kullan�lan bir algoritmad�r. Her istek Concurrency s�n�r�n� bir azalmakta ve bittikleri taktirde bu s�n�r� bir artt�rmaktad�rlar.
//Di�er algoritmalara nazaran sadece asenkron requestleri s�n�rland�r�r.
builder.Services.AddRateLimiter(options =>
{
    options.AddConcurrencyLimiter("Concurrency", _options =>
    {
        _options.PermitLimit = 4; //Her bir periottaki limit 4 tane 
        _options.QueueLimit = 2; 
        _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});
#endregion

#region OnRejected Propery'si
//E�er fazla istek att�kta sonra fazla at�lan istekler �zerinde i�lemler yap�lmak isteniyorsa �rnek olarak loglama onregected property kullan�l�r
//builder.Services.AddRateLimiter(options =>
//{
//    options.AddFixedWindowLimiter("Basic", _options =>
//    {
//        _options.Window = TimeSpan.FromSeconds(12);
//        _options.PermitLimit = 4;
//        _options.QueueLimit = 2;
//        _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//    });

//    options.OnRejected = (context, CancellationToken) =>
//    {
//        //logging veya ba�ka i�lemler
//        return new();
//    };

//});
#endregion

#region
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy<string, CustomRateLimitPolicy>("CustomRateLimit");

});
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//Minimal Api i�in rate limit uygulamak
//app.MapGet("/",() => {

//}).RequireRateLimiting("Concurrency");

app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
