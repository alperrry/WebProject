using Microsoft.EntityFrameworkCore;

public class LibrarySeedData
{
    public static async void SeedData(IApplicationBuilder app)
    {
        var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<LibraryDbContext>();

        if (context != null)
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                 context.Database.Migrate();
            }

           
            if (!context.categories.Any())
            {
                
                var categories = new List<Category>
                {
                    new Category { CategoryId = 000, Name = " Bilgisayar bilimi, enformatik & genel çalışmalar" },
                    new Category { CategoryId = 100, Name = " Felsefe ve psikoloji" },
                    new Category { CategoryId = 200, Name = " Din" },
                    new Category { CategoryId = 300, Name = " Sosyal bilimler" },
                    new Category { CategoryId = 400, Name = " Dil" },
                    new Category { CategoryId = 500, Name = " Temel bilimler" },
                    new Category { CategoryId = 600, Name = " Teknoloji" },
                    new Category { CategoryId = 700, Name = " Sanat ve yaratıcılık" },
                    new Category { CategoryId = 800, Name = " Edebiyat" },
                    new Category { CategoryId = 900, Name = " Tarih ve coğrafya" }
                };

                await context.categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            if (!context.books.Any())
            {
               
                var books = new List<Books>
                {
                   
                    new Books { Name = "Introduction to Computer Science", CategoryId = 000 },
                    new Books { Name = "Computer Networks", CategoryId = 000 },
                    new Books { Name = "Artificial Intelligence", CategoryId = 000 },
                    new Books { Name = "Data Structures and Algorithms", CategoryId = 000 },
                    new Books { Name = "Operating Systems", CategoryId = 000 },

                   
                    new Books { Name = "The Republic", CategoryId = 100 },
                    new Books { Name = "Meditations", CategoryId = 100 },
                    new Books { Name = "Man's Search for Meaning", CategoryId = 100 },
                    new Books { Name = "Psychology: The Science of Behavior", CategoryId = 100 },
                    new Books { Name = "The Psychology of Human Behavior", CategoryId = 100 },

                   
                    new Books { Name = "The Bible", CategoryId = 200 },
                    new Books { Name = "The Quran", CategoryId = 200 },
                    new Books { Name = "Bhagavad Gita", CategoryId = 200 },
                    new Books { Name = "The Tao Te Ching", CategoryId = 200 },
                    new Books { Name = "The Dhammapada", CategoryId = 200 },

                    
                    new Books { Name = "Sociology: A Global Perspective", CategoryId = 300 },
                    new Books { Name = "The Sociological Imagination", CategoryId = 300 },
                    new Books { Name = "The Social Animal", CategoryId = 300 },
                    new Books { Name = "Social Problems", CategoryId = 300 },
                    new Books { Name = "Introduction to Sociology", CategoryId = 300 },

                   
                    new Books { Name = "The Elements of Style", CategoryId = 400 },
                    new Books { Name = "Language Universals", CategoryId = 400 },
                    new Books { Name = "The Power of Babel", CategoryId = 400 },
                    new Books { Name = "Word Power Made Easy", CategoryId = 400 },
                    new Books { Name = "The Penguin Guide to Punctuation", CategoryId = 400 },

                    
                    new Books { Name = "Physics: Principles with Applications", CategoryId = 500 },
                    new Books { Name = "Biology: A Guide to the Natural World", CategoryId = 500 },
                    new Books { Name = "Chemistry: The Central Science", CategoryId = 500 },
                    new Books { Name = "Introduction to Quantum Mechanics", CategoryId = 500 },
                    new Books { Name = "The Feynman Lectures on Physics", CategoryId = 500 },

                   
                    new Books { Name = "The Innovators", CategoryId = 600 },
                    new Books { Name = "The Lean Startup", CategoryId = 600 },
                    new Books { Name = "The Phoenix Project", CategoryId = 600 },
                    new Books { Name = "Code Complete", CategoryId = 600 },
                    new Books { Name = "Clean Code", CategoryId = 600 },

                    
                    new Books { Name = "The Creative Habit", CategoryId = 700 },
                    new Books { Name = "Steal Like an Artist", CategoryId = 700 },
                    new Books { Name = "The War of Art", CategoryId = 700 },
                    new Books { Name = "Big Magic", CategoryId = 700 },
                    new Books { Name = "Art & Fear", CategoryId = 700 },

                    
                    new Books { Name = "To Kill a Mockingbird", CategoryId = 800 },
                    new Books { Name = "1984", CategoryId = 800 },
                    new Books { Name = "Pride and Prejudice", CategoryId = 800 },
                    new Books { Name = "The Great Gatsby", CategoryId = 800 },
                    new Books { Name = "Moby Dick", CategoryId = 800 },

                    
                    new Books { Name = "Sapiens: A Brief History of Humankind", CategoryId = 900 },
                    new Books { Name = "Guns, Germs, and Steel", CategoryId = 900 },
                    new Books { Name = "A People's History of the United States", CategoryId = 900 },
                    new Books { Name = "The History of the Ancient World", CategoryId = 900 },
                    new Books { Name = "The Silk Roads", CategoryId = 900 }
                };

                await context.books.AddRangeAsync(books);
                await context.SaveChangesAsync();
            }
        }
    }
}
