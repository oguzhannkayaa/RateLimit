using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region Fixed Window
//Sabit bir zaman aralýðý kullanarak istekleri sýnýrlandýran algoritmadýr.
//builder.Services.AddRateLimiter(options => {
//    options.AddFixedWindowLimiter("Basic", _options =>
//    {
//        _options.Window = TimeSpan.FromSeconds(12); //her 12 saniyede
//        _options.PermitLimit = 4; // 4 tane request gönderme
//        _options.QueueLimit = 2; //4 tane fazla olursa 2 sini queue ya al daha fazlasýný boþluða al
//        _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; //process etmeye eskiden baþla
//    });
//});
#endregion

#region Sliding Window
//Bulunduðu periodun yarýsýnda sonra 
//builder.Services.AddRateLimiter(options => {
//    options.AddSlidingWindowLimiter("Sliding",_options =>
//    {
//        _options.Window = TimeSpan.FromSeconds(12); //12 saniyede bir çalýþmasý gerektiði
//        _options.PermitLimit = 4; //her 12 saniyede 4 tane request iþlemesi
//        _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//        _options.QueueLimit = 2;
//        _options.SegmentsPerWindow = 2; //bir sonraki windowun en fazla 2 tanesini kullanabildiðini göstermek için her periot bir önceki sýradan 2 tane harcama olasýðý versin

//    });
//});
#endregion

#region Token Butcket
//Her bir periotta iþlecek request sayýsý kadar token üretmektedir.
//Eðer bu tokenlar kullanýldýysa diðer periottan borç alýnabilir. Her periotta token üretim miktari kadar token üretilicek 
//bu þekilde rate limit uygulacaktýr. Her periodun maximum token limit verilen sabir sayý kadar olacaktýr

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
//Asencron requestleri sýnýrlandýrmak için kullanýlan bir algoritmadýr. Her istek Concurrency sýnýrýný bir azalmakta ve bittikleri taktirde bu sýnýrý bir arttýrmaktadýrlar.
//Diðer algoritmalara nazaran sadece asenkron requestleri sýnýrlandýrýr.
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
//Eðer fazla istek attýkta sonra fazla atýlan istekler üzerinde iþlemler yapýlmak isteniyorsa örnek olarak loglama onregected property kullanýlýr
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
//        //logging veya baþka iþlemler
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
//Minimal Api için rate limit uygulamak
//app.MapGet("/",() => {

//}).RequireRateLimiting("Concurrency");

app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
