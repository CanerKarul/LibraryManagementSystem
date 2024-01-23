using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

// Kullanıcı sınıfı
class User
{
    public string Username { get; set; }
    public string Password { get; set; }

    //kullanıcı girişi
    public static User Register()
    {
        Console.WriteLine("\n*************************************\n");
        Console.WriteLine("Kullanıcı Kaydı");
        Console.WriteLine("Kullanıcı Adı: ");
        string username = Console.ReadLine();
        Console.WriteLine("Şifre: ");
        string password = Console.ReadLine();
        Console.WriteLine("\n*************************************\n");

        return new User { Username = username, Password = password };
    } 
}

// Kitap Sınıfı
class Book
{
    public string Title { get; set; } 
    public string Author { get; set; } 
    public string ISBN { get; set; } 
    public int CopyCount { get; set; }
    public int BorrowedCopies { get; set; }
    public DateTime DueDate { get; set; }

    public Book() 
    {
        DueDate = DateTime.Now.AddDays(14); // DueDate geçerli tarihten 14 gün sonrasına ayarlandı
    }

}

class Library
{
    private List<User> users;
    private List<Book> books;
    private User loggedInUser; // Şu anki oturum açmış kullanıcı

    public Library()
    {
        users = new List<User>();
        books = new List<Book>();
        LoadUsersFromDataFile();
        LoadBooksFromDataFile();
    }

    // users.txt dosyasından kullanıcı verilerini yüklemek
    private void LoadUsersFromDataFile()
    {
        try
        {
            string[] lines = File.ReadAllLines("users.txt");
            foreach (var line in lines)
            {
                string[] userData = line.Split(',');
                User user = new User
                {
                    Username = userData[0],
                    Password = userData[1]
                };
                users.Add(user);
            }
        }
        catch (FileNotFoundException)
        {
            // Dosya bulunamazsa, daha sonra yeni bir dosya oluşturulacaktır.
        }
        catch (Exception ex)
        {
            CenterText("Kullanıcı verileri yüklenirken bir hata oluştu: " + ex.Message);
        }
    }

    // kitaplar.txt dosyasından kitap verilerini yüklemek
    private void LoadBooksFromDataFile()
    {
        try
        {
            string[] lines = File.ReadAllLines("kitaplar.txt");
            foreach (var line in lines)
            {
                string[] bookData = line.Split(',');
                Book book = new Book
                {
                    Title = bookData[0],
                    Author = bookData[1],
                    ISBN = bookData[2],
                    CopyCount = int.Parse(bookData[3]),
                    BorrowedCopies = int.Parse(bookData[4]),
                    DueDate = DateTime.Parse(bookData[5])
                };

                books.Add(book);

            }
        }
        catch (FileNotFoundException)
        {
            // Dosya bulunamazsa, daha sonra yeni dosya oluşturulacaktır
        }
        catch (Exception ex)
        {
            CenterText("Kitap verileri yüklenirken bir hata oluştu: " + ex.Message);
        }
    }

    // Kullanıcı verilerini "users.txt" dosyasına kaydetmek
    private void SaveUsersToDataFile()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter("users.txt"))
            {
                foreach (var user in users)
                {
                    writer.WriteLine($"{user.Username},{user.Password}");
                }
            }
        }
        catch(Exception ex)
        {
            CenterText("Kullanıcı verileri kaydedilirken bir hata oluştu: " + ex.Message);
        }
    }

    // Kitap verilerini "kitaplar.txt" dosyasına kaydetmek
    private void SaveBooksToDataFile()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter("kitaplar.txt"))
            {
                foreach(var book in books)
                {
                    writer.WriteLine($"{book.Title},{book.Author},{book.ISBN},{book.CopyCount},{book.BorrowedCopies},{book.DueDate.ToString("F")}");
                }
            }
        }
        catch (Exception ex)
        {
            CenterText("Kitap verileri kaydedilirken bir hata oluştu: " + ex.Message);
        }
    }

    // Kütüphane sistemi
    public void RunLibrary()
    {
        CenterText("!!!Kütüphane Sistemine Hoş Geldiniz!!!");

        //Kullanıcıların var olup olmadığını kontrol et 
        if (users.Count == 0)
        {
            CenterText("Henüz kayıtlı kullanıcı buılunmamaktadır. Lütfen kayıt olun.");
            User newUser = User.Register();
            users.Add(newUser);
            SaveUsersToDataFile();
        }

        loggedInUser = Login();

        CenterText($"Giriş başarılı. Hoş geldiniz, {loggedInUser.Username}");

        while (true)
        {
            Console.WriteLine("\nKÜTÜPHANE MENÜSÜ");
            Console.WriteLine("1. Kitapları Listele");
            Console.WriteLine("2. Kitap Ekle");
            Console.WriteLine("3. Kitap Sil");
            Console.WriteLine("4. Kitap Ödünç Al");
            Console.WriteLine("5. Kitap İade Et");
            Console.WriteLine("6. Çıkış");

            Console.Write("Seçiminizi Yapınız: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                    case "1":
                    ListBooks();
                    break;

                    case "2":
                    AddBook();
                    break;

                    case "3":
                    RemoveBook();
                    break; 
                    
                    case "4":
                    BorrowBook();
                    break;

                    case "5":
                    ReturnBook();
                    break;

                    case "6":
                    SaveBooksToDataFile();
                    Environment.Exit(0);
                    break;

                    default: Console.WriteLine("Geçersiz seçenek. Tekrar deneyin!");
                    break;

            }

        }

    }
    
    // Kullanıcı girişini yöneten yöntem
    private User Login()
    {
        User user = null;

        while (user == null)
        {
            Console.WriteLine("\n*************************************\n");
            Console.WriteLine("Kullanıcı Girişi");
            Console.WriteLine("Kullanıcı Adı: ");
            string username = Console.ReadLine();
            Console.WriteLine("Şifre: ");
            string password = Console.ReadLine();
            Console.WriteLine("\n*************************************\n");

            user = users.Find(u => u.Username == username && u.Password == password);

            if(user == null)
            {
                Console.WriteLine("Giriş Başarısız. Lütfen tekrar deneyin.");
            }
        }
        return user;

    }

    // Tüm kitapları listeleyen yöntem
    private void ListBooks()
    {
        CenterText("Kitap Listesi");
        foreach (var book in books)
        {
            Console.WriteLine("\n*************************************\n");
            Console.WriteLine($"Kitabın İsmi: {book.Title} \nKitabın Yazarı: {book.Author} \nISBN No: {book.ISBN} \nKopya Sayısı: {book.CopyCount} \nÖdünç Alınan Kopya Sayısı: {book.BorrowedCopies}\n");
            Console.WriteLine("\n*************************************\n");
        }
    }

    // Yeni kitap ekleyen yöntem
    private void AddBook()
    {
        CenterText("Yeni Kitap Ekle");
        Console.WriteLine("Kitap Başlığı: ");
        string title = Console.ReadLine();
        Console.WriteLine("Yazar: ");
        string author = Console.ReadLine();
        Console.WriteLine("ISBN No: ");
        string isbn = Console.ReadLine();
        Console.WriteLine("Kopya Sayısını Giriniz: ");
        int copyCount = int.Parse(Console.ReadLine());

        Book newBook = new Book
        {
            Title = title,
            Author = author,
            ISBN = isbn,
            CopyCount = copyCount,
            BorrowedCopies = 0
        };

        books.Add(newBook);
        SaveBooksToDataFile();
        Console.WriteLine("Kitap Eklendi");
        
    }

    // Kütüphaneden silme yöntemi
    private void RemoveBook()
    {
        CenterText("Kitap Sil");
        Console.WriteLine("Silinecek Kitabın ISBN No Gir: ");
        string isbnToRemove = Console.ReadLine();
        Book bookToRemove = books.Find(book  => book.ISBN == isbnToRemove);

        if (bookToRemove != null)
        {
            books.Remove(bookToRemove);
            SaveBooksToDataFile();
            Console.WriteLine("Kitap Silindi.");
        }
        else Console.WriteLine("Kitap Bulunamadı!");
    }

    // Kitap ödünç alma yöntemi
    private void BorrowBook()
    {
        CenterText("Kitap Ödünç Al");
        Console.WriteLine("Ödünç Alınacak Kitabın ISBN Numarasını Girin: ");
        string isbnToBorrow = Console.ReadLine();
        Book bookToBorrow = books.Find(book => book.ISBN == isbnToBorrow);

        if (bookToBorrow != null)
        {
            if (bookToBorrow.BorrowedCopies < bookToBorrow.CopyCount)
            {
                bookToBorrow.BorrowedCopies++;
                SaveBooksToDataFile();
                Console.WriteLine($"Kitap ödünç alındı. İade tarihi: {bookToBorrow.DueDate.ToString("F")}");
            }
            else Console.WriteLine("Alınacak kopya kalmadı. Başka kitaplara bakabilirsin!");
        }
        else Console.WriteLine("Kitap Bulunamadı!");
    }

    // Kitap iade etme yöntemi
    private void ReturnBook()
    {
        CenterText("Kitap İade Êt");
        Console.WriteLine("İade Edilecek Kitabın ISBN Numarasını Giriniz: ");
        string isbnToReturn = Console.ReadLine();
        Book bookToReturn = books.Find(book => book.ISBN == isbnToReturn);

        if (bookToReturn != null && bookToReturn.BorrowedCopies > 0)
        {
            if (DateTime.Now <= bookToReturn.DueDate)
            {
                bookToReturn.BorrowedCopies--;
                Console.WriteLine($"Kitap Zamanında İade Edildi. Son tarih: {bookToReturn.DueDate.ToString("F")}");
            }
            else Console.WriteLine("!!!Kitap süresinde iade edilmedi!!!");
            

            SaveBooksToDataFile();
        }
        else Console.WriteLine("Kitap bulunamadı veya zaten iade edilmiş.");
        

    }

    // Metni ortalama
    private static void CenterText(string text)
    {
        Console.SetCursorPosition((Console.WindowWidth - text.Length) / 2, Console.CursorTop);
        Console.WriteLine(text);
    }

}


class Program
{
    // Kütüphane programını başlatmak için ana yöntem
    static void Main()
    {
        Library library = new Library();
        library.RunLibrary();
    }
}

