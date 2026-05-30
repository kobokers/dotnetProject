using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project.Models;

namespace project;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (await context.Posts.AnyAsync()) return;

        var users = new List<ApplicationUser>
        {
            new()
            {
                UserName = "akmal",
                Email = "akmal@mehkawan.com",
                DisplayName = "Akmal Rizki",
                Bio = "Full-stack developer & UI/UX designer. Building the future one line at a time.",
                Location = "Jakarta, Indonesia",
                ProfilePhoto = "https://ui-avatars.com/api/?name=Akmal+Rizki&background=e05555&color=fff&size=200",
                CoverPhoto = null,
                FriendCode = "akmal#4821",
                CreatedAt = DateTime.UtcNow.AddDays(-90),
                LastSeen = DateTime.UtcNow.AddMinutes(-5)
            },
            new()
            {
                UserName = "sarah",
                Email = "sarah@mehkawan.com",
                DisplayName = "Sarah Chen",
                Bio = "Digital artist & photographer. Capturing moments that matter.",
                Location = "Seoul, South Korea",
                ProfilePhoto = "https://ui-avatars.com/api/?name=Sarah+Chen&background=7c3aed&color=fff&size=200",
                CoverPhoto = null,
                FriendCode = "sarah#7732",
                CreatedAt = DateTime.UtcNow.AddDays(-85),
                LastSeen = DateTime.UtcNow.AddMinutes(-12)
            },
            new()
            {
                UserName = "alex",
                Email = "alex@mehkawan.com",
                DisplayName = "Alex Rivera",
                Bio = "Startup founder. Previously @google, @meta. Tweets about tech & life.",
                Location = "San Francisco, CA",
                ProfilePhoto = "https://ui-avatars.com/api/?name=Alex+Rivera&background=00b894&color=fff&size=200",
                CoverPhoto = null,
                FriendCode = "alex#9054",
                CreatedAt = DateTime.UtcNow.AddDays(-80),
                LastSeen = DateTime.UtcNow.AddMinutes(-2)
            },
            new()
            {
                UserName = "maya",
                Email = "maya@mehkawan.com",
                DisplayName = "Maya Patel",
                Bio = "Product designer @figma. Writing about design systems and creativity.",
                Location = "London, UK",
                ProfilePhoto = "https://ui-avatars.com/api/?name=Maya+Patel&background=f91880&color=fff&size=200",
                CoverPhoto = null,
                FriendCode = "maya#6612",
                CreatedAt = DateTime.UtcNow.AddDays(-75),
                LastSeen = DateTime.UtcNow.AddMinutes(-30)
            },
            new()
            {
                UserName = "jordan",
                Email = "jordan@mehkawan.com",
                DisplayName = "Jordan Kim",
                Bio = "ML Engineer @openai. I teach machines to dream.",
                Location = "Tokyo, Japan",
                ProfilePhoto = "https://ui-avatars.com/api/?name=Jordan+Kim&background=4f46e5&color=fff&size=200",
                CoverPhoto = null,
                FriendCode = "jordan#3348",
                CreatedAt = DateTime.UtcNow.AddDays(-70),
                LastSeen = DateTime.UtcNow.AddMinutes(-8)
            },
            new()
            {
                UserName = "nina",
                Email = "nina@mehkawan.com",
                DisplayName = "Nina Okafor",
                Bio = "UX researcher & accessibility advocate. Making the web work for everyone.",
                Location = "Lagos, Nigeria",
                ProfilePhoto = "https://ui-avatars.com/api/?name=Nina+Okafor&background=ff6b35&color=fff&size=200",
                CoverPhoto = null,
                FriendCode = "nina#2209",
                CreatedAt = DateTime.UtcNow.AddDays(-65),
                LastSeen = DateTime.UtcNow.AddMinutes(-1)
            }
        };

        foreach (var user in users)
        {
            await userManager.CreateAsync(user, "Password123!");
        }

        var akmal = users[0];
        var sarah = users[1];
        var alex = users[2];
        var maya = users[3];
        var jordan = users[4];
        var nina = users[5];

        var friendPairs = new[]
        {
            (akmal, sarah), (akmal, alex), (akmal, maya),
            (sarah, alex), (sarah, jordan),
            (alex, maya), (alex, jordan), (alex, nina),
            (maya, nina),
            (jordan, nina)
        };

        var random = new Random(42);
        foreach (var (sender, receiver) in friendPairs)
        {
            var (senderId, receiverId) = random.Next(2) == 0 ? (sender.Id, receiver.Id) : (receiver.Id, sender.Id);
            context.FriendRequests.Add(new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Status = FriendRequestStatus.Accepted,
                CreatedAt = DateTime.UtcNow.AddDays(-60)
            });
        }

        await context.SaveChangesAsync();

        var postData = new[]
        {
            new { User = akmal, Content = "Just shipped the new MehKawan update! Dark mode, real-time chat, and a brand new UI. The team crushed it 🔥", ImageUrl = (string?)"https://picsum.photos/seed/mehkawan/600/400", CreatedAt = DateTime.UtcNow.AddHours(-2) },
            new { User = akmal, Content = "Hot take: TypeScript is the best thing that happened to JavaScript. Fight me.", ImageUrl = (string?)null, CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new { User = akmal, Content = "Working on something exciting for the community. Stay tuned 🚀", ImageUrl = (string?)null, CreatedAt = DateTime.UtcNow.AddDays(-3) },
            new { User = sarah, Content = "Golden hour at Gangnam. Seoul never disappoints 📸", ImageUrl = (string?)"https://picsum.photos/seed/seoul/600/400", CreatedAt = DateTime.UtcNow.AddHours(-5) },
            new { User = sarah, Content = "New art series dropping next week! Here's a sneak peek ✨", ImageUrl = (string?)"https://picsum.photos/seed/art/600/400", CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new { User = alex, Content = "5 years ago I quit my FAANG job. Best decision I ever made. Here's what I learned:", ImageUrl = (string?)null, CreatedAt = DateTime.UtcNow.AddHours(-8) },
            new { User = alex, Content = "Just closed our Series A! 🎉 Grateful for the team and investors who believed in our vision.", ImageUrl = (string?)null, CreatedAt = DateTime.UtcNow.AddDays(-4) },
            new { User = alex, Content = "Startup tip: Your first 100 users matter more than your first $100k. Build for people, not profits.", ImageUrl = (string?)null, CreatedAt = DateTime.UtcNow.AddDays(-7) },
            new { User = maya, Content = "Design systems are not just component libraries. They're communication tools that scale with your organization.", ImageUrl = (string?)"https://picsum.photos/seed/design/600/400", CreatedAt = DateTime.UtcNow.AddHours(-3) },
            new { User = maya, Content = "Typography is 90% of design. Everything else is decoration.", ImageUrl = (string?)null, CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new { User = jordan, Content = "We're open-sourcing our internal ML training framework! Check the repo link in bio 🧠", ImageUrl = (string?)null, CreatedAt = DateTime.UtcNow.AddHours(-6) },
            new { User = jordan, Content = "The future of AI is not about making models bigger. It's about making them more accessible and efficient.", ImageUrl = (string?)null, CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new { User = nina, Content = "Accessibility is not a feature. It's a fundamental right. Every product should be usable by everyone.", ImageUrl = (string?)null, CreatedAt = DateTime.UtcNow.AddHours(-4) },
            new { User = nina, Content = "Just ran a UX workshop with 50 students in Lagos. The energy was incredible! 🌍", ImageUrl = (string?)"https://picsum.photos/seed/lagos/600/400", CreatedAt = DateTime.UtcNow.AddDays(-3) },
            new { User = sarah, Content = "Coffee and sketching. The perfect Sunday morning ☕✏️", ImageUrl = (string?)"https://picsum.photos/seed/coffee/600/400", CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new { User = maya, Content = "Just redesigned our entire component library. 40% less CSS, 100% more consistent.", ImageUrl = (string?)null, CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new { User = jordan, Content = "Paper published! Our research on efficient transformers is now live. Link in bio.", ImageUrl = (string?)null, CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new { User = alex, Content = "Reading 'The Mom Test' by @robfitz. Every founder should read this before talking to customers.", ImageUrl = (string?)null, CreatedAt = DateTime.UtcNow.AddDays(-1) },
        };

        var posts = new List<Post>();
        foreach (var p in postData)
        {
            var post = new Post
            {
                UserId = p.User.Id,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                CreatedAt = p.CreatedAt
            };
            context.Posts.Add(post);
            posts.Add(post);
        }

        await context.SaveChangesAsync();

        var commentData = new[]
        {
            new { Post = posts[0], User = sarah, Content = "This update is amazing! Love the new dark mode 🔥", CreatedAt = DateTime.UtcNow.AddHours(-1) },
            new { Post = posts[0], User = alex, Content = "The UI is so clean now. Great work team!", CreatedAt = DateTime.UtcNow.AddMinutes(-45) },
            new { Post = posts[0], User = maya, Content = "The chat feature is everything. Finally!", CreatedAt = DateTime.UtcNow.AddMinutes(-30) },
            new { Post = posts[1], User = jordan, Content = "Not a hot take, it's facts. TypeScript saved my sanity.", CreatedAt = DateTime.UtcNow.AddHours(-12) },
            new { Post = posts[1], User = nina, Content = "TypeScript with strict mode is chef's kiss 🤌", CreatedAt = DateTime.UtcNow.AddHours(-11) },
            new { Post = posts[3], User = akmal, Content = "Seoul looks incredible! Adding it to my bucket list 📸", CreatedAt = DateTime.UtcNow.AddHours(-3) },
            new { Post = posts[3], User = maya, Content = "The lighting in this shot is perfect!", CreatedAt = DateTime.UtcNow.AddHours(-2) },
            new { Post = posts[5], User = akmal, Content = "Inspiring story Alex! Would love to hear more about the journey.", CreatedAt = DateTime.UtcNow.AddHours(-6) },
            new { Post = posts[5], User = nina, Content = "This is exactly what I needed to hear today 🙏", CreatedAt = DateTime.UtcNow.AddHours(-5) },
            new { Post = posts[5], User = jordan, Content = "Leaving FAANG was the scariest and best thing I ever did too.", CreatedAt = DateTime.UtcNow.AddHours(-4) },
            new { Post = posts[8], User = alex, Content = "This is so true. We learned this the hard way at my startup.", CreatedAt = DateTime.UtcNow.AddHours(-1) },
            new { Post = posts[8], User = akmal, Content = "Saving this for my team meeting tomorrow!", CreatedAt = DateTime.UtcNow.AddMinutes(-50) },
            new { Post = posts[13], User = sarah, Content = "Would love to collaborate on a workshop sometime! 🤝", CreatedAt = DateTime.UtcNow.AddHours(-2) },
            new { Post = posts[13], User = alex, Content = "This is the kind of work that actually changes lives. Keep it up!", CreatedAt = DateTime.UtcNow.AddHours(-1) },
        };

        foreach (var c in commentData)
        {
            context.Comments.Add(new Comment
            {
                PostId = c.Post.PostId,
                UserId = c.User.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt
            });
        }

        await context.SaveChangesAsync();

        foreach (var post in posts)
        {
            var likers = users.Where(u => u.Id != post.UserId).OrderBy(_ => random.Next()).Take(random.Next(2, 5));
            foreach (var liker in likers)
            {
                if (!context.Likes.Any(l => l.PostId == post.PostId && l.UserId == liker.Id))
                {
                    context.Likes.Add(new Like
                    {
                        PostId = post.PostId,
                        UserId = liker.Id,
                        CreatedAt = post.CreatedAt.AddMinutes(random.Next(1, 120))
                    });
                }
            }
        }

        await context.SaveChangesAsync();

        foreach (var (user1, user2) in new[] { (akmal, sarah), (akmal, alex), (sarah, maya), (alex, jordan), (jordan, nina) })
        {
            var messageTime = DateTime.UtcNow.AddDays(-random.Next(1, 14));
            context.Messages.Add(new Message
            {
                SenderId = user1.Id,
                ReceiverId = user2.Id,
                Content = $"Hey {user2.DisplayName}, how's it going?",
                CreatedAt = messageTime,
                IsRead = random.Next(0, 2) == 1
            });
            context.Messages.Add(new Message
            {
                SenderId = user2.Id,
                ReceiverId = user1.Id,
                Content = $"Hey {user1.DisplayName}! Doing great, thanks for asking!",
                CreatedAt = messageTime.AddMinutes(5),
                IsRead = random.Next(0, 2) == 1
            });
        }

        await context.SaveChangesAsync();

        foreach (var user in users)
        {
            var from = users.Where(u => u.Id != user.Id).OrderBy(_ => random.Next()).First();
            context.Notifications.Add(new Notification
            {
                UserId = user.Id,
                Type = NotificationType.FriendAccepted,
                FromUserId = from.Id,
                Message = $"{from.DisplayName} accepted your friend request",
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                IsRead = random.Next(0, 2) == 1
            });
        }

        await context.SaveChangesAsync();

        foreach (var user in new[] { sarah, alex, maya })
        {
            context.Stories.Add(new Story
            {
                UserId = user.Id,
                ImageUrl = "https://picsum.photos/seed/story" + user.Id[..4] + "/420/700",
                CreatedAt = DateTime.UtcNow.AddHours(-random.Next(1, 12))
            });
        }

        await context.SaveChangesAsync();
    }
}
