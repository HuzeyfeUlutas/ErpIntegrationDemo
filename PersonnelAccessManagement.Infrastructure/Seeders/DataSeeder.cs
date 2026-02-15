using Bogus;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Domain.Entities;
using PersonnelAccessManagement.Domain.Enums;
using PersonnelAccessManagement.Persistence.DbContexts;

namespace PersonnelAccessManagement.Infrastructure.Seeders;

public class DataSeeder
{
    private readonly PersonnelAccessManagementDbContext _context; // kendi DbContext adınla değiştir

    private static readonly string[] FirstNames = {
        "Ahmet", "Mehmet", "Mustafa", "Ali", "Hüseyin", "Hasan", "İbrahim", "Ömer",
        "Yusuf", "Murat", "İsmail", "Osman", "Ramazan", "Süleyman", "Halil", "Mahmut",
        "Recep", "Salih", "Fatih", "Kadir", "Emre", "Burak", "Serkan", "Onur",
        "Cem", "Kerem", "Tolga", "Volkan", "Barış", "Erkan", "Gökhan", "Uğur",
        "Tuncay", "Adem", "Ferhat", "Yasin", "Furkan", "Enes", "Berkay", "Kaan",
        "Eren", "Yiğit", "Arda", "Berat", "Emir", "Mert", "Efe", "Deniz",
        "Caner", "Engin", "Selim", "Tarık", "Sinan", "Erdem", "Alper", "Serhat",
        "Oğuz", "Taha", "Umut", "Koray", "Levent", "Aykut", "Soner", "Cenk",
        "Ayşe", "Fatma", "Emine", "Hatice", "Zeynep", "Elif", "Merve", "Büşra",
        "Esra", "Zehra", "Havva", "Meryem", "Şerife", "Sultan", "Fadime",
        "Hanife", "Seda", "Gül", "Özlem", "Derya", "Sibel", "Pınar", "Sevgi",
        "Gamze", "Tuğba", "Ebru", "Dilek", "Hülya", "Filiz", "Aslı", "Cansu",
        "İrem", "Nur", "Buse", "Gizem", "Betül", "Melek", "Selin", "Damla",
        "Şeyma", "Aleyna", "Ecrin", "Defne", "Yağmur", "Cemre", "Dilan", "Ceyda",
        "Nisa", "Nehir", "Ada", "Mina", "Azra", "Lina", "Asya", "Ela"
    };

    private static readonly string[] LastNames = {
        "Yılmaz", "Kaya", "Demir", "Çelik", "Şahin", "Yıldız", "Yıldırım", "Öztürk",
        "Aydın", "Özdemir", "Arslan", "Doğan", "Kılıç", "Aslan", "Çetin", "Koç",
        "Kurt", "Özkan", "Şimşek", "Polat", "Korkmaz", "Aktaş", "Acar",
        "Tekin", "Aksoy", "Çınar", "Güneş", "Kara", "Erdoğan", "Bulut", "Taş",
        "Kaplan", "Ateş", "Güler", "Bozkurt", "Tunç", "Başaran", "Erdem", "Karaca",
        "Uçar", "Sarı", "Keskin", "Duman", "Işık", "Bayrak", "Türk", "Karakuş",
        "Sezer", "Coşkun", "Gündüz", "Sönmez", "Avcı", "Karataş", "Ünal", "Uysal",
        "Bayram", "Peker", "Çakır", "Toprak", "Ceylan", "Duran", "Ergin", "Yavuz",
        "Köse", "Akın", "Altın", "Balcı", "Tanrıverdi", "Elmas", "Bostancı",
        "Karadağ", "Soylu", "Dinç", "Gökçe", "Akbaş", "Albayrak", "Turan", "Ekinci",
        "Yalçın", "Demirci", "Şen", "Aydoğan", "Bakır", "Özer", "Biçer", "Bağcı",
        "Dursun", "Eroğlu", "Genç", "Usta", "Arıkan", "Çiftçi", "Korkut", "Tokgöz"
    };

    public DataSeeder(PersonnelAccessManagementDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        // ── 1) Roller zaten varsa atla ──────────────────
        if (!await _context.Set<Role>().AnyAsync(ct))
        {
            var roleNames = GenerateTurnikeGroupNames(350);
            var roleId = 1;

            foreach (var name in roleNames)
            {
                var role = new Role(roleId, name);
                role.CreatedAt = DateTime.UtcNow;
                role.CreatedBy = "Seeder";

                _context.Set<Role>().Add(role);
                await _context.SaveChangesAsync(ct);
                _context.ChangeTracker.Clear();
                roleId++;
            }
        }

        // ── 2) Personeller zaten varsa atla ─────────────
        if (!await _context.Set<Personnel>().AnyAsync(ct))
        {
            var campuses = Enum.GetValues<Campus>();
            var titles = Enum.GetValues<Title>();
            var random = new Random(42);
            var usedEmployeeNos = new HashSet<decimal>();

            for (var batch = 0; batch < 13; batch++)
            {
                var personnels = new List<Personnel>(1000);

                for (var i = 0; i < 1000; i++)
                {
                    decimal empNo;
                    do { empNo = random.Next(10000, 99999); }
                    while (!usedEmployeeNos.Add(empNo));

                    var fullName = $"{FirstNames[random.Next(FirstNames.Length)]} {LastNames[random.Next(LastNames.Length)]}";

                    var personnel = new Personnel(
                        empNo,
                        fullName,
                        campuses[random.Next(campuses.Length)],
                        titles[random.Next(titles.Length)]
                    );
                    personnel.CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 730));
                    personnel.CreatedBy = "Seeder";

                    personnels.Add(personnel);
                }

                await _context.Set<Personnel>().AddRangeAsync(personnels, ct);
                await _context.SaveChangesAsync(ct);
                _context.ChangeTracker.Clear();
            }
        }
    }

    private static List<string> GenerateTurnikeGroupNames(int count)
    {
        var names = new HashSet<string>();

        var buildings = new[]
        {
            "Ana Bina", "Yönetim Binası", "Üretim Binası", "Depo Binası",
            "Ar-Ge Binası", "Laboratuvar Binası", "Sosyal Tesis",
            "Ek Bina", "Teknik Bina", "Lojistik Binası",
            "Bakım Binası", "Eğitim Binası", "Kalite Binası",
            "IT Binası", "Enerji Binası", "Atölye Binası",
            "Montaj Binası", "Paketleme Binası", "Sevkiyat Binası",
            "Yemekhane", "Misafirhane", "Sağlık Merkezi",
        };

        var blocks = new[] { "A Blok", "B Blok", "C Blok", "D Blok", "E Blok" };
        var floors = new[] { "Zemin Kat", "1. Kat", "2. Kat", "3. Kat", "4. Kat", "Bodrum" };
        var zones = new[] { "Kuzey Giriş", "Güney Giriş", "Doğu Giriş", "Batı Giriş", "Ana Giriş", "Arka Giriş", "Yan Giriş" };
        var areas = new[] { "Otopark", "Bahçe", "Teras", "Çatı", "Koridor" };
        var campusNames = new[] { "İstanbul", "Ankara", "İzmir" };

        foreach (var building in buildings)
        foreach (var block in blocks)
            names.Add($"{building} - {block}");

        foreach (var building in buildings)
        foreach (var floor in floors)
            names.Add($"{building} - {floor}");

        foreach (var building in buildings)
        foreach (var zone in zones)
            names.Add($"{building} - {zone}");

        foreach (var campus in campusNames)
        foreach (var area in areas)
            names.Add($"{campus} - {area}");

        for (var i = 1; i <= 30; i++)
            names.Add($"Bina {i}");

        for (var i = 1; i <= 20; i++)
            names.Add($"Üretim Hattı {i}");

        for (var i = 1; i <= 10; i++)
            names.Add($"Depo {i}");

        var extraPrefixes = new[] { "Güvenlikli Alan", "Kısıtlı Bölge", "Özel Alan", "Kontrollü Giriş" };
        var counter = 1;
        while (names.Count < count)
        {
            var prefix = extraPrefixes[counter % extraPrefixes.Length];
            names.Add($"{prefix} - {counter}");
            counter++;
        }

        return names.Take(count).ToList();
    }
}