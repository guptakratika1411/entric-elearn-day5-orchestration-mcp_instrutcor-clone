using System.Globalization;
using LearnHub.Api.Models;

namespace LearnHub.Api.Data;

public class InMemoryStore
{
    public List<Course> Courses { get; } = new()
    {
        new Course
        {
            Id = 1, Price = 0m, Title = "Intro to C# Programming", ShortDescription = "Learn the basics of C# by writing small programs.",
            Category = "Programming", Instructor = "Maya Rao", Level = "Beginner",
            Lessons =
            {
                new Lesson { Id = 1, Title = "Setting up your tools", DurationMinutes = 10, Content = "Install the .NET SDK and run your first program." },
                new Lesson { Id = 2, Title = "Variables and types", DurationMinutes = 15, Content = "Store numbers, text and true/false values." },
                new Lesson { Id = 3, Title = "Conditions and loops", DurationMinutes = 20, Content = "Make decisions and repeat work with if and for." },
                new Lesson { Id = 4, Title = "Methods", DurationMinutes = 15, Content = "Group code into small reusable pieces." },
            }
        },
        new Course
        {
            Id = 2, Price = 40m, Title = "JavaScript for the Web", ShortDescription = "Build interactive web pages with modern JavaScript.",
            Category = "Programming", Instructor = "Liam Carter", Level = "Intermediate",
            Lessons =
            {
                new Lesson { Id = 5, Title = "The DOM", DurationMinutes = 15, Content = "Find and change things on a web page." },
                new Lesson { Id = 6, Title = "Events", DurationMinutes = 12, Content = "React to clicks and key presses." },
                new Lesson { Id = 7, Title = "Fetching data", DurationMinutes = 18, Content = "Call an API with fetch and show the result." },
                new Lesson { Id = 8, Title = "Async code", DurationMinutes = 20, Content = "Understand promises and async/await." },
            }
        },
        new Course
        {
            Id = 3, Price = 30m, Title = "UI Design Basics", ShortDescription = "Learn layout, colour and type for clean interfaces.",
            Category = "Design", Instructor = "Sofia Alvarez", Level = "Beginner",
            Lessons =
            {
                new Lesson { Id = 9, Title = "Layout and spacing", DurationMinutes = 12, Content = "Use a grid and consistent spacing." },
                new Lesson { Id = 10, Title = "Colour", DurationMinutes = 10, Content = "Pick an accent colour and keep contrast high." },
                new Lesson { Id = 11, Title = "Typography", DurationMinutes = 12, Content = "Choose readable fonts and sizes." },
            }
        },
        new Course
        {
            Id = 4, Price = 30m, Title = "Data Analysis with SQL", ShortDescription = "Ask questions of data using SQL queries.",
            Category = "Data", Instructor = "Ken Watanabe", Level = "Intermediate",
            Lessons =
            {
                new Lesson { Id = 12, Title = "SELECT basics", DurationMinutes = 15, Content = "Read rows from a table." },
                new Lesson { Id = 13, Title = "Filtering and sorting", DurationMinutes = 15, Content = "Use WHERE and ORDER BY." },
                new Lesson { Id = 14, Title = "Joins", DurationMinutes = 20, Content = "Combine data from two tables." },
                new Lesson { Id = 15, Title = "Grouping", DurationMinutes = 18, Content = "Count and total with GROUP BY." },
                new Lesson { Id = 16, Title = "Query practice", DurationMinutes = 25, Content = "Solve small real-world questions." },
            }
        },
        new Course
        {
            Id = 5, Price = 0m, Title = "Cloud Fundamentals", ShortDescription = "Understand servers, storage and networks in the cloud.",
            Category = "Cloud", Instructor = "Priya Nair", Level = "Beginner",
            Lessons =
            {
                new Lesson { Id = 17, Title = "What is the cloud?", DurationMinutes = 10, Content = "Compute, storage and networking in plain words." },
                new Lesson { Id = 18, Title = "Virtual machines", DurationMinutes = 15, Content = "Start a server and connect to it." },
                new Lesson { Id = 19, Title = "Storage", DurationMinutes = 12, Content = "Files, blocks and databases." },
                new Lesson { Id = 20, Title = "Cost basics", DurationMinutes = 10, Content = "Understand what you pay for." },
            }
        },
        new Course
        {
            Id = 6, Price = 60m, Title = "Deploying Apps with Containers", ShortDescription = "Package an app in a container and run it anywhere.",
            Category = "Cloud", Instructor = "Omar Hassan", Level = "Advanced",
            Lessons =
            {
                new Lesson { Id = 21, Title = "Why containers?", DurationMinutes = 10, Content = "The problem containers solve." },
                new Lesson { Id = 22, Title = "Writing a Dockerfile", DurationMinutes = 20, Content = "Describe how to build your app image." },
                new Lesson { Id = 23, Title = "Running containers", DurationMinutes = 15, Content = "Start, stop and inspect containers." },
                new Lesson { Id = 24, Title = "Compose", DurationMinutes = 20, Content = "Run several containers together." },
            }
        },
    };

    public List<Enrollment> Enrollments { get; } = new();
    public List<LessonProgress> Progress { get; } = new();
    public List<Bookmark> Bookmarks { get; } = new();
    private int _nextBookmarkId = 1;

    // ---- Shop data ----

    /// <summary>The store's clock. Tests replace it so dates are fixed.</summary>
    public Func<DateTime> Clock { get; set; } = () => DateTime.UtcNow;

    public List<CartItem> Cart { get; } = new();
    public List<Order> Orders { get; } = new();

    public List<Coupon> Coupons { get; } = new()
    {
        new Coupon { Code = "WELCOME10", Type = "Percent", Value = 10, ExpiresOn = new DateTime(2099, 12, 31), MaxUses = 1000 },
        new Coupon { Code = "SAVE20", Type = "Fixed", Value = 20, ExpiresOn = new DateTime(2099, 12, 31), MaxUses = 1000, MinOrderAmount = 50 },
        new Coupon { Code = "CLOUD25", Type = "Percent", Value = 25, ExpiresOn = new DateTime(2099, 12, 31), MaxUses = 100, Category = "Cloud" },
        new Coupon { Code = "FLAT50", Type = "Fixed", Value = 50, ExpiresOn = new DateTime(2099, 12, 31), MaxUses = 100 },
        new Coupon { Code = "OLD5", Type = "Percent", Value = 5, ExpiresOn = new DateTime(2024, 12, 31), MaxUses = 100 },
    };

    private int _nextEnrollmentId = 1;
    private int _nextOrderId = 1;

    public List<QuizQuestion> QuizQuestions { get; } = new()
    {
        Q(1, 1, "Which tool do you install to run C# programs?", 0, "The .NET SDK", "Photoshop", "A web browser", "Excel"),
        Q(2, 1, "What does 'dotnet run' do?", 1, "Deletes your project", "Builds and starts your program", "Installs Windows", "Opens a website"),
        Q(3, 2, "Which type stores whole numbers?", 2, "string", "bool", "int", "char"),
        Q(4, 2, "Which type stores true or false?", 0, "bool", "int", "double", "string"),
        Q(5, 3, "Which keyword repeats code a set number of times?", 1, "if", "for", "class", "using"),
        Q(6, 3, "What does 'if' do?", 1, "Repeats code", "Makes a decision", "Names a method", "Imports a library"),
        Q(7, 4, "Why use methods?", 1, "To make code slower", "To group code into reusable pieces", "To delete variables", "To change colours"),
        Q(8, 4, "Which word means a method returns nothing?", 2, "null", "empty", "void", "none"),
        Q(9, 5, "What does DOM stand for?", 0, "Document Object Model", "Data Output Mode", "Digital Order Map", "Display Object Menu"),
        Q(10, 5, "Which call finds an element by its id?", 0, "document.getElementById", "document.makeId", "window.find", "page.select"),
        Q(11, 6, "Which event fires when a button is clicked?", 1, "hover", "click", "load", "scroll"),
        Q(12, 6, "What does addEventListener do?", 0, "Runs code when an event happens", "Adds a CSS class", "Deletes an element", "Starts a server"),
        Q(13, 7, "Which function calls an API from the browser?", 0, "fetch", "send", "pull", "load"),
        Q(14, 7, "What format do most APIs return?", 0, "JSON", "MP3", "PDF", "ZIP"),
        Q(15, 8, "What does 'await' do?", 1, "Stops the browser", "Waits for a promise to finish", "Deletes a promise", "Repeats a loop"),
        Q(16, 8, "Which keyword marks a function that can use await?", 0, "async", "defer", "later", "wait"),
        Q(17, 9, "What helps keep a layout tidy?", 1, "Random spacing", "A grid and consistent spacing", "Many fonts", "Tiny text"),
        Q(18, 9, "What is white space good for?", 1, "Wasting room", "Making content easier to read", "Hiding content", "Slowing the page"),
        Q(19, 10, "What should you keep high for text colour?", 0, "Contrast", "Noise", "Saturation only", "Opacity only"),
        Q(20, 10, "How many accent colours should a simple design start with?", 0, "One", "Ten", "Twenty", "None"),
        Q(21, 11, "What makes text readable?", 0, "Clear fonts and good size", "Very small text", "Many styles at once", "Light grey on white"),
        Q(22, 11, "Which is a good body text size on the web?", 1, "4px", "16px", "60px", "2px"),
    };

    private static QuizQuestion Q(int id, int lessonId, string question, int correctIndex, params string[] options) =>
        new() { Id = id, LessonId = lessonId, Question = question, CorrectIndex = correctIndex, Options = options.ToList() };

    // ---- Courses ----

    public IEnumerable<Course> SearchCourses(string? search) =>
        Courses.Where(c => string.IsNullOrWhiteSpace(search) ||
            c.Title.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase));

    // ---- Enrollment ----

    public bool IsEnrolled(string studentName, int courseId) =>
        Enrollments.Any(e => e.CourseId == courseId &&
            string.Equals(e.StudentName, studentName.Trim(), StringComparison.OrdinalIgnoreCase));

    /// <summary>Returns the new enrollment, or an error text ("required", "course", "duplicate").</summary>
    public (Enrollment? Enrollment, string? Error) Enroll(int courseId, string? studentName)
    {
        var name = studentName?.Trim();
        if (string.IsNullOrEmpty(name)) return (null, "studentName is required.");
        if (!Courses.Any(c => c.Id == courseId)) return (null, "Course not found.");
        if (IsEnrolled(name, courseId)) return (null, "Already enrolled in this course.");

        var enrollment = new Enrollment { Id = _nextEnrollmentId++, CourseId = courseId, StudentName = name };
        Enrollments.Add(enrollment);
        return (enrollment, null);
    }

    /// <summary>Direct enrollment is only for free courses. Paid courses go through the cart.</summary>
    public (Enrollment? Enrollment, string? Error) EnrollFree(int courseId, string? studentName)
    {
        var course = Courses.FirstOrDefault(c => c.Id == courseId);
        if (course is not null && course.Price > 0) return (null, "This course is paid. Add it to your cart and check out.");
        return Enroll(courseId, studentName);
    }

    // ---- Quiz ----

    public List<QuizQuestion> GetQuiz(int lessonId) =>
        QuizQuestions.Where(q => q.LessonId == lessonId).ToList();

    public (int Score, int Total, bool Passed) ScoreQuiz(int lessonId, IEnumerable<(int QuestionId, int SelectedIndex)> answers)
    {
        var questions = GetQuiz(lessonId);
        var score = questions.Count(q => answers.Any(a => a.QuestionId == q.Id && a.SelectedIndex == q.CorrectIndex));
        var total = questions.Count;
        return (score, total, total > 0 && score * 2 >= total);
    }

    // ---- Progress ----

    /// <summary>Marks a lesson done. Safe to call twice. Returns an error text or null.</summary>
    public string? MarkLessonComplete(string? studentName, int courseId, int lessonId)
    {
        var name = studentName?.Trim();
        if (string.IsNullOrEmpty(name)) return "studentName is required.";
        var course = Courses.FirstOrDefault(c => c.Id == courseId);
        if (course is null) return "Course not found.";
        if (!course.Lessons.Any(l => l.Id == lessonId)) return "Lesson not found in this course.";
        if (!IsEnrolled(name, courseId)) return "Student is not enrolled in this course.";

        var done = Progress.Any(p => p.CourseId == courseId && p.LessonId == lessonId &&
            string.Equals(p.StudentName, name, StringComparison.OrdinalIgnoreCase));
        if (!done)
            Progress.Add(new LessonProgress { StudentName = name, CourseId = courseId, LessonId = lessonId });
        return null;
    }

    public List<(int CourseId, string CourseTitle, int CompletedLessons, int TotalLessons, int Percent, List<int> CompletedLessonIds)> GetProgress(string? studentName)
    {
        var name = studentName?.Trim() ?? "";
        return Enrollments
            .Where(e => string.Equals(e.StudentName, name, StringComparison.OrdinalIgnoreCase))
            .Select(e =>
            {
                var course = Courses.First(c => c.Id == e.CourseId);
                var ids = Progress
                    .Where(p => p.CourseId == course.Id && string.Equals(p.StudentName, name, StringComparison.OrdinalIgnoreCase))
                    .Select(p => p.LessonId).ToList();
                var total = course.Lessons.Count;
                var percent = total == 0 ? 0 : (int)Math.Round(100.0 * ids.Count / total);
                return (course.Id, course.Title, ids.Count, total, percent, ids);
            }).ToList();
    }

    // ---- Cart ----

    private const decimal BundleRate = 0.15m;
    private const int BundleMinCourses = 3;
    private const decimal InstructorShareRate = 0.70m;
    private const int RefundDays = 7;
    private const double RefundMaxProgressPercent = 30;

    private static decimal Money(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

    private static string Fmt(decimal value) => value.ToString("0.00", CultureInfo.InvariantCulture);

    /// <summary>Returns the new cart item, or an error text.</summary>
    public (CartItem? Item, string? Error) AddToCart(string? studentName, int courseId)
    {
        var name = studentName?.Trim();
        if (string.IsNullOrEmpty(name)) return (null, "studentName is required.");
        var course = Courses.FirstOrDefault(c => c.Id == courseId);
        if (course is null) return (null, "Course not found.");
        if (course.Price <= 0) return (null, "This course is free. Enroll in it directly.");
        if (IsEnrolled(name, courseId)) return (null, "You already own this course.");
        if (IsInCart(name, courseId)) return (null, "Already in your cart.");

        var item = new CartItem { StudentName = name, CourseId = courseId, AddedAt = Clock() };
        Cart.Add(item);
        return (item, null);
    }

    /// <summary>Returns true if an item was removed.</summary>
    public bool RemoveFromCart(string? studentName, int courseId)
    {
        var name = studentName?.Trim() ?? "";
        return Cart.RemoveAll(i => i.CourseId == courseId &&
            string.Equals(i.StudentName, name, StringComparison.OrdinalIgnoreCase)) > 0;
    }

    public bool IsInCart(string studentName, int courseId) =>
        Cart.Any(i => i.CourseId == courseId &&
            string.Equals(i.StudentName, studentName.Trim(), StringComparison.OrdinalIgnoreCase));

    public List<CartItem> GetCart(string? studentName)
    {
        var name = studentName?.Trim() ?? "";
        return Cart
            .Where(i => string.Equals(i.StudentName, name, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    // ---- Prices: bundle and coupon ----

    /// <summary>Works out the price of the student's cart. Bundle first, then the coupon. Changes nothing.</summary>
    public PriceQuote Quote(string? studentName, string? couponCode)
    {
        var name = studentName?.Trim() ?? "";
        var quote = new PriceQuote
        {
            Lines = GetCart(name).Select(i =>
            {
                var course = Courses.First(c => c.Id == i.CourseId);
                return new OrderLine { CourseId = course.Id, Title = course.Title, Instructor = course.Instructor, Price = course.Price };
            }).ToList()
        };
        var lines = quote.Lines;

        // 1. Bundle: 3 or more paid courses get 15% off every line.
        if (lines.Count >= BundleMinCourses)
            foreach (var line in lines) line.BundleDiscount = Money(line.Price * BundleRate);

        // 2. Coupon, worked out on the prices after the bundle.
        var code = couponCode?.Trim();
        if (!string.IsNullOrEmpty(code) && lines.Count > 0)
        {
            var error = ApplyCoupon(name, code, quote, out var applied);
            if (error is not null)
            {
                quote.CouponRejected = true;
                quote.CouponMessage = error;
            }
            else
            {
                quote.CouponCode = applied!.Code;
                quote.CouponMessage = applied.Type == "Percent"
                    ? $"Coupon {applied.Code} applied: {applied.Value.ToString("0.##", CultureInfo.InvariantCulture)}% off."
                    : $"Coupon {applied.Code} applied: {Fmt(applied.Value)} off.";
            }
        }

        foreach (var line in lines)
            line.Final = Math.Max(0, line.Price - line.BundleDiscount - line.CouponDiscount);
        quote.Subtotal = lines.Sum(l => l.Price);
        quote.BundleDiscount = lines.Sum(l => l.BundleDiscount);
        quote.CouponDiscount = lines.Sum(l => l.CouponDiscount);
        quote.Total = Math.Max(0, lines.Sum(l => l.Final));
        return quote;
    }

    /// <summary>Checks the coupon and fills CouponDiscount on the lines it applies to. Returns an error text or null.</summary>
    private string? ApplyCoupon(string studentName, string code, PriceQuote quote, out Coupon? applied)
    {
        applied = null;
        var coupon = Coupons.FirstOrDefault(c => string.Equals(c.Code, code, StringComparison.OrdinalIgnoreCase));
        if (coupon is null) return "Coupon not found.";
        if (coupon.ExpiresOn.Date < Clock().Date) return "This coupon has expired.";
        if (coupon.UsedCount >= coupon.MaxUses) return "This coupon has no uses left.";
        if (HasUsedCoupon(studentName, coupon.Code)) return "You have already used this coupon.";

        var afterBundle = quote.Lines.Sum(l => l.Price - l.BundleDiscount);
        if (afterBundle < coupon.MinOrderAmount)
            return $"This coupon needs an order of at least {Fmt(coupon.MinOrderAmount)} after the bundle discount.";

        var eligible = quote.Lines.Where(l =>
            string.IsNullOrEmpty(coupon.Category) ||
            string.Equals(Courses.First(c => c.Id == l.CourseId).Category, coupon.Category, StringComparison.OrdinalIgnoreCase)).ToList();
        if (eligible.Count == 0) return $"This coupon only works for {coupon.Category} courses.";

        var eligibleTotal = eligible.Sum(l => l.Price - l.BundleDiscount);
        if (coupon.Type == "Percent")
        {
            foreach (var line in eligible)
                line.CouponDiscount = Math.Min(line.Price - line.BundleDiscount, Money((line.Price - line.BundleDiscount) * coupon.Value / 100m));
        }
        else if (eligibleTotal > 0)
        {
            // A fixed amount never goes above what the matching lines cost. It is shared by line price.
            var discount = Math.Min(coupon.Value, eligibleTotal);
            var given = 0m;
            for (var i = 0; i < eligible.Count; i++)
            {
                var net = eligible[i].Price - eligible[i].BundleDiscount;
                eligible[i].CouponDiscount = i == eligible.Count - 1 ? discount - given : Money(discount * net / eligibleTotal);
                given += eligible[i].CouponDiscount;
            }
        }
        applied = coupon;
        return null;
    }

    /// <summary>A student may use each coupon once. A refunded order does not count.</summary>
    public bool HasUsedCoupon(string studentName, string code) =>
        Orders.Any(o => o.Status == "Paid" && o.CouponCode is not null &&
            string.Equals(o.CouponCode, code, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(o.StudentName, studentName.Trim(), StringComparison.OrdinalIgnoreCase));

    // ---- Checkout and orders ----

    /// <summary>Re-checks everything, then saves the order. Returns the order, or an error text.</summary>
    public (Order? Order, string? Error) Checkout(string? studentName, string? couponCode)
    {
        var name = studentName?.Trim();
        if (string.IsNullOrEmpty(name)) return (null, "studentName is required.");
        var cart = GetCart(name);
        if (cart.Count == 0) return (null, "Your cart is empty.");
        foreach (var item in cart)
            if (IsEnrolled(name, item.CourseId))
                return (null, $"You already own {Courses.First(c => c.Id == item.CourseId).Title}. Remove it from your cart.");

        var quote = Quote(name, couponCode);
        if (quote.CouponRejected) return (null, quote.CouponMessage);

        var order = new Order
        {
            Id = _nextOrderId++,
            StudentName = name,
            CreatedAt = Clock(),
            Lines = quote.Lines,
            CouponCode = quote.CouponCode,
            Subtotal = quote.Subtotal,
            BundleDiscount = quote.BundleDiscount,
            CouponDiscount = quote.CouponDiscount,
            Total = quote.Total,
            Status = "Paid"
        };
        Orders.Add(order);

        if (quote.CouponCode is not null)
            Coupons.First(c => string.Equals(c.Code, quote.CouponCode, StringComparison.OrdinalIgnoreCase)).UsedCount++;
        foreach (var line in order.Lines) Enroll(line.CourseId, name);
        Cart.RemoveAll(i => string.Equals(i.StudentName, name, StringComparison.OrdinalIgnoreCase));
        return (order, null);
    }

    public List<Order> GetOrders(string? studentName)
    {
        var name = studentName?.Trim() ?? "";
        return Orders
            .Where(o => string.Equals(o.StudentName, name, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(o => o.Id)
            .ToList();
    }

    // ---- Refunds ----

    /// <summary>Refunds an order. Returns the order, or an error text ("Order not found.", "This order was already refunded.", or the reason).</summary>
    public (Order? Order, string? Error) RefundOrder(int orderId)
    {
        var order = Orders.FirstOrDefault(o => o.Id == orderId);
        if (order is null) return (null, "Order not found.");
        if (order.Status == "Refunded") return (null, "This order was already refunded.");
        if (Clock() - order.CreatedAt > TimeSpan.FromDays(RefundDays))
            return (null, $"Refunds are only possible within {RefundDays} days of the order.");

        foreach (var line in order.Lines)
        {
            var percent = ProgressPercent(order.StudentName, line.CourseId);
            if (percent >= RefundMaxProgressPercent)
                return (null, $"You have finished {Math.Round(percent)}% of {line.Title}. Refunds need progress under {RefundMaxProgressPercent}% on every course.");
        }

        order.Status = "Refunded";
        foreach (var line in order.Lines)
        {
            Enrollments.RemoveAll(e => e.CourseId == line.CourseId &&
                string.Equals(e.StudentName, order.StudentName, StringComparison.OrdinalIgnoreCase));
            Progress.RemoveAll(p => p.CourseId == line.CourseId &&
                string.Equals(p.StudentName, order.StudentName, StringComparison.OrdinalIgnoreCase));
        }
        if (order.CouponCode is not null)
        {
            var coupon = Coupons.FirstOrDefault(c => string.Equals(c.Code, order.CouponCode, StringComparison.OrdinalIgnoreCase));
            if (coupon is not null && coupon.UsedCount > 0) coupon.UsedCount--;
        }
        return (order, null);
    }

    private double ProgressPercent(string studentName, int courseId)
    {
        var total = Courses.First(c => c.Id == courseId).Lessons.Count;
        if (total == 0) return 0;
        var done = Progress.Count(p => p.CourseId == courseId &&
            string.Equals(p.StudentName, studentName, StringComparison.OrdinalIgnoreCase));
        return 100.0 * done / total;
    }

    // ---- Instructor revenue ----

    /// <summary>One row per instructor. The share is 70% of the paid sales that were not refunded.</summary>
    public List<(string Instructor, decimal GrossSales, decimal Refunds, decimal InstructorShare)> GetInstructorRevenue() =>
        Courses.Select(c => c.Instructor).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(i => i).Select(instructor =>
        {
            var lines = Orders
                .SelectMany(o => o.Lines.Select(l => (o.Status, Line: l)))
                .Where(x => string.Equals(x.Line.Instructor, instructor, StringComparison.OrdinalIgnoreCase))
                .ToList();
            var gross = lines.Sum(x => x.Line.Final);
            var refunds = lines.Where(x => x.Status == "Refunded").Sum(x => x.Line.Final);
            return (instructor, gross, refunds, Money((gross - refunds) * InstructorShareRate));
        }).ToList();

    // ---- Bookmarks ----

    /// <summary>Returns the new bookmark, or an error text.</summary>
    public (Bookmark? Bookmark, string? Error) AddBookmark(string? studentName, int courseId)
    {
        var name = studentName?.Trim();
        if (string.IsNullOrEmpty(name)) return (null, "studentName is required.");
        if (!Courses.Any(c => c.Id == courseId)) return (null, "Course not found.");
        if (IsBookmarked(name, courseId)) return (null, "Already bookmarked.");

        var bookmark = new Bookmark { Id = _nextBookmarkId++, StudentName = name, CourseId = courseId };
        Bookmarks.Add(bookmark);
        return (bookmark, null);
    }

    /// <summary>Returns true if a bookmark was removed.</summary>
    public bool RemoveBookmark(string? studentName, int courseId)
    {
        var name = studentName?.Trim() ?? "";
        return Bookmarks.RemoveAll(b => b.CourseId == courseId &&
            string.Equals(b.StudentName, name, StringComparison.OrdinalIgnoreCase)) > 0;
    }

    public bool IsBookmarked(string studentName, int courseId) =>
        Bookmarks.Any(b => b.CourseId == courseId &&
            string.Equals(b.StudentName, studentName.Trim(), StringComparison.OrdinalIgnoreCase));

    public List<Bookmark> GetBookmarks(string? studentName)
    {
        var name = studentName?.Trim() ?? "";
        return Bookmarks
            .Where(b => string.Equals(b.StudentName, name, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    // ---- Certificates ----

    /// <summary>A certificate is earned when progress for a course is 100%.</summary>
    public List<(string CourseTitle, string StudentName, DateTime IssuedOn, string CertificateId)> GetCertificates(string? studentName)
    {
        var name = studentName?.Trim() ?? "";
        return GetProgress(name)
            .Where(p => p.TotalLessons > 0 && p.Percent == 100)
            .Select(p =>
            {
                var issuedOn = Progress
                    .Where(x => x.CourseId == p.CourseId && string.Equals(x.StudentName, name, StringComparison.OrdinalIgnoreCase))
                    .Max(x => x.CompletedAt);
                var student = Enrollments.First(e => e.CourseId == p.CourseId &&
                    string.Equals(e.StudentName, name, StringComparison.OrdinalIgnoreCase)).StudentName;
                return (p.CourseTitle, student, issuedOn, $"LH-{p.CourseId:D3}-{student.ToUpperInvariant().Replace(" ", "")}");
            }).ToList();
    }

    // ---- Instructor: add course ----

    private static readonly string[] Levels = { "Beginner", "Intermediate", "Advanced" };

    /// <summary>Adds a course (and its lessons) with new unique ids. Returns the course, or an error text.</summary>
    public (Course? Course, string? Error) AddCourse(Course draft)
    {
        if (string.IsNullOrWhiteSpace(draft.Title)) return (null, "title is required.");
        if (string.IsNullOrWhiteSpace(draft.ShortDescription)) return (null, "shortDescription is required.");
        if (string.IsNullOrWhiteSpace(draft.Category)) return (null, "category is required.");
        if (string.IsNullOrWhiteSpace(draft.Instructor)) return (null, "instructor is required.");
        var level = Levels.FirstOrDefault(l => string.Equals(l, draft.Level?.Trim(), StringComparison.OrdinalIgnoreCase));
        if (level is null) return (null, "level must be Beginner, Intermediate or Advanced.");
        if (draft.Price < 0) return (null, "price cannot be below 0.");
        if (draft.Lessons.Count == 0) return (null, "At least one lesson is required.");
        if (draft.Lessons.Any(l => string.IsNullOrWhiteSpace(l.Title))) return (null, "Every lesson needs a title.");
        if (draft.Lessons.Any(l => l.DurationMinutes <= 0)) return (null, "Every lesson needs durationMinutes above 0.");
        if (Courses.Any(c => string.Equals(c.Title, draft.Title.Trim(), StringComparison.OrdinalIgnoreCase)))
            return (null, "A course with this title already exists.");

        var nextLessonId = Courses.SelectMany(c => c.Lessons).Max(l => l.Id) + 1;
        var course = new Course
        {
            Id = Courses.Max(c => c.Id) + 1,
            Title = draft.Title.Trim(),
            ShortDescription = draft.ShortDescription.Trim(),
            Category = draft.Category.Trim(),
            Instructor = draft.Instructor.Trim(),
            Level = level,
            Price = draft.Price,
            Lessons = draft.Lessons.Select(l => new Lesson
            {
                Id = nextLessonId++,
                Title = l.Title.Trim(),
                DurationMinutes = l.DurationMinutes,
                Content = l.Content?.Trim() ?? ""
            }).ToList()
        };
        Courses.Add(course);
        return (course, null);
    }
}
