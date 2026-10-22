using LearnHub.Api.Data;
using LearnHub.Api.Models;

namespace LearnHub.Tests;

// One test (or a small group) for each rule in FEATURE.md. The clock is fixed so dates never change.
// Seed prices: course 2 = 40, course 3 = 30, course 4 = 30, course 6 = 60. Courses 1 and 5 are free.
public class ShopTests
{
    private static readonly DateTime Today = new(2026, 3, 1, 12, 0, 0, DateTimeKind.Utc);

    private static InMemoryStore NewStore()
    {
        var store = new InMemoryStore();
        store.Clock = () => Today;
        return store;
    }

    private static void AddToCart(InMemoryStore store, string name, params int[] courseIds)
    {
        foreach (var id in courseIds) Assert.NotNull(store.AddToCart(name, id).Item);
    }

    private static void CompleteLessons(InMemoryStore store, string name, int courseId, int count)
    {
        foreach (var lesson in store.Courses.First(c => c.Id == courseId).Lessons.Take(count))
            store.MarkLessonComplete(name, courseId, lesson.Id);
    }

    // R1
    [Fact]
    public void R1_EveryCourseHasAPrice_AndAtLeastTwoAreFree()
    {
        var store = NewStore();
        Assert.All(store.Courses, c => Assert.True(c.Price >= 0));
        Assert.True(store.Courses.Count(c => c.Price == 0) >= 2);
        Assert.True(store.Courses.Count(c => c.Price > 0) >= 3);
    }

    // R2
    [Fact]
    public void R2_FreeCourseEnrollsDirectly_PaidCourseDoesNot()
    {
        var store = NewStore();
        Assert.NotNull(store.EnrollFree(1, "Asha").Enrollment);
        var paid = store.EnrollFree(2, "Asha");
        Assert.Null(paid.Enrollment);
        Assert.Contains("paid", paid.Error);
    }

    // R3
    [Fact]
    public void R3_Cart_HoldsManyCourses_PerStudent_AndRemoves()
    {
        var store = NewStore();
        AddToCart(store, "Asha", 2, 3);
        AddToCart(store, "Ben", 4);

        Assert.Equal(new[] { 2, 3 }, store.GetCart("asha").Select(i => i.CourseId));
        Assert.Single(store.GetCart("Ben"));
        Assert.True(store.RemoveFromCart("ASHA", 2));
        Assert.False(store.RemoveFromCart("Asha", 2));
        Assert.Single(store.GetCart("Asha"));
    }

    // R4
    [Fact]
    public void R4_FreeCourse_CannotBeAddedToCart()
    {
        var store = NewStore();
        var result = store.AddToCart("Asha", 1);
        Assert.Null(result.Item);
        Assert.Equal("This course is free. Enroll in it directly.", result.Error);
    }

    // R5
    [Fact]
    public void R5_OwnedCourse_AndDuplicate_AreRejected()
    {
        var store = NewStore();
        AddToCart(store, "Asha", 2);
        Assert.Equal("Already in your cart.", store.AddToCart("ASHA", 2).Error);

        store.Checkout("Asha", null);
        Assert.Equal("You already own this course.", store.AddToCart("Asha", 2).Error);
    }

    // R6
    [Fact]
    public void R6_Bundle_GivesFifteenPercentOff_OnlyForThreeOrMoreCourses()
    {
        var store = NewStore();
        AddToCart(store, "Asha", 2, 3);
        var two = store.Quote("Asha", null);
        Assert.Equal(0m, two.BundleDiscount);
        Assert.Equal(70m, two.Total);

        AddToCart(store, "Asha", 4);
        var three = store.Quote("Asha", null);
        Assert.Equal(100m, three.Subtotal);
        Assert.Equal(15m, three.BundleDiscount);
        Assert.Equal(85m, three.Total);
        Assert.Equal(6m, three.Lines.First(l => l.CourseId == 2).BundleDiscount);
    }

    // R7
    [Fact]
    public void R7_Coupons_ArePercentOrFixed_AndCodeIgnoresCase()
    {
        var store = NewStore();
        AddToCart(store, "Asha", 2);
        var percent = store.Quote("Asha", "welcome10");
        Assert.Equal(4m, percent.CouponDiscount);
        Assert.Equal(36m, percent.Total);

        store.Coupons.Add(new Coupon { Code = "FIVE", Type = "Fixed", Value = 5, ExpiresOn = Today.AddDays(30), MaxUses = 10 });
        var fixedOff = store.Quote("Asha", "Five");
        Assert.Equal(5m, fixedOff.CouponDiscount);
        Assert.Equal(35m, fixedOff.Total);
    }

    // R8
    [Fact]
    public void R8_Coupon_IsRejected_WhenUnknownExpiredOrUsedUp()
    {
        var store = NewStore();
        store.Coupons.Add(new Coupon { Code = "LASTDAY", Type = "Percent", Value = 10, ExpiresOn = Today.Date, MaxUses = 10 });
        store.Coupons.Add(new Coupon { Code = "FULL", Type = "Percent", Value = 10, ExpiresOn = Today.AddDays(5), MaxUses = 2, UsedCount = 2 });
        AddToCart(store, "Asha", 2);

        var unknown = store.Quote("Asha", "NOPE");
        Assert.True(unknown.CouponRejected);
        Assert.Equal("Coupon not found.", unknown.CouponMessage);
        Assert.Equal("This coupon has expired.", store.Quote("Asha", "OLD5").CouponMessage);
        Assert.Equal("This coupon has no uses left.", store.Quote("Asha", "FULL").CouponMessage);
        Assert.False(store.Quote("Asha", "LASTDAY").CouponRejected); // still valid on its last day

        store.Clock = () => Today.AddDays(1);
        Assert.Equal("This coupon has expired.", store.Quote("Asha", "LASTDAY").CouponMessage);
    }

    // R9
    [Fact]
    public void R9_Coupon_CanBeUsedOnlyOncePerStudent()
    {
        var store = NewStore();
        AddToCart(store, "Asha", 2);
        Assert.NotNull(store.Checkout("Asha", "WELCOME10").Order);

        AddToCart(store, "Asha", 3);
        Assert.Equal("You have already used this coupon.", store.Checkout("Asha", "WELCOME10").Error);

        AddToCart(store, "Ben", 3);
        Assert.NotNull(store.Checkout("Ben", "WELCOME10").Order);
    }

    // R10
    [Fact]
    public void R10_BundleComesFirst_ThenCoupon_AndMinimumIsCheckedAfterBundle()
    {
        var store = NewStore();
        AddToCart(store, "Asha", 2, 3, 4);
        var quote = store.Quote("Asha", "WELCOME10");
        Assert.Equal(100m, quote.Subtotal);
        Assert.Equal(15m, quote.BundleDiscount);
        Assert.Equal(8.50m, quote.CouponDiscount);
        Assert.Equal(76.50m, quote.Total);

        // 100 before the bundle, but only 85 after it. A minimum of 90 must fail.
        store.Coupons.Add(new Coupon { Code = "MIN90", Type = "Percent", Value = 10, ExpiresOn = Today.AddDays(30), MaxUses = 10, MinOrderAmount = 90 });
        Assert.True(store.Quote("Asha", "MIN90").CouponRejected);
        Assert.Contains("at least 90.00", store.Quote("Asha", "MIN90").CouponMessage);
    }

    // R11
    [Fact]
    public void R11_CategoryCoupon_OnlyDiscountsMatchingLines()
    {
        var store = NewStore();
        AddToCart(store, "Asha", 2, 3, 6); // 40 + 30 + 60 = 130. Only course 6 is Cloud.
        var quote = store.Quote("Asha", "cloud25");

        Assert.Equal(19.50m, quote.BundleDiscount);
        Assert.Equal(0m, quote.Lines.First(l => l.CourseId == 2).CouponDiscount);
        Assert.Equal(12.75m, quote.Lines.First(l => l.CourseId == 6).CouponDiscount);
        Assert.Equal(97.75m, quote.Total);

        AddToCart(store, "Ben", 2);
        var none = store.Quote("Ben", "CLOUD25");
        Assert.True(none.CouponRejected);
        Assert.Equal("This coupon only works for Cloud courses.", none.CouponMessage);
    }

    // R12
    [Fact]
    public void R12_Total_NeverGoesBelowZero()
    {
        var store = NewStore();
        AddToCart(store, "Asha", 3); // 30, and FLAT50 takes 50 off
        var quote = store.Quote("Asha", "FLAT50");
        Assert.Equal(30m, quote.CouponDiscount);
        Assert.Equal(0m, quote.Total);
        Assert.All(quote.Lines, l => Assert.True(l.Final >= 0));

        var order = store.Checkout("Asha", "FLAT50").Order;
        Assert.Equal(0m, order!.Total);
        Assert.True(store.IsEnrolled("Asha", 3));
    }

    // R13
    [Fact]
    public void R13_Money_RoundsToTwoDecimals_HalfAwayFromZero()
    {
        var store = NewStore();
        store.Courses.Add(new Course { Id = 7, Title = "Penny course", Category = "Test", Instructor = "Maya Rao", Price = 0.01m, Lessons = { new Lesson { Id = 100, Title = "One", DurationMinutes = 5 } } });
        store.Coupons.Add(new Coupon { Code = "HALF", Type = "Percent", Value = 50, ExpiresOn = Today.AddDays(30), MaxUses = 10 });
        AddToCart(store, "Asha", 7);

        var quote = store.Quote("Asha", "HALF"); // 0.005 rounds up to 0.01
        Assert.Equal(0.01m, quote.CouponDiscount);
        Assert.Equal(0m, quote.Total);

        AddToCart(store, "Ben", 2, 3, 4);
        var ben = store.Quote("Ben", "WELCOME10"); // 3.4 + 2.55 + 2.55
        Assert.Equal(new[] { 3.40m, 2.55m, 2.55m }, ben.Lines.Select(l => l.CouponDiscount));
    }

    // R14
    [Fact]
    public void R14_Preview_ShowsEachLine_AndChangesNothing()
    {
        var store = NewStore();
        AddToCart(store, "Asha", 2, 3, 4);
        var quote = store.Quote("Asha", "WELCOME10");

        Assert.Equal(3, quote.Lines.Count);
        var line = quote.Lines.First(l => l.CourseId == 2);
        Assert.Equal((40m, 6m, 3.40m, 30.60m), (line.Price, line.BundleDiscount, line.CouponDiscount, line.Final));
        Assert.Contains("WELCOME10", quote.CouponMessage);

        Assert.Equal(3, store.GetCart("Asha").Count);
        Assert.Empty(store.Orders);
        Assert.Equal(0, store.Coupons.First(c => c.Code == "WELCOME10").UsedCount);
    }

    // R15
    [Fact]
    public void R15_Checkout_RejectsEmptyCart_BadCoupon_AndCoursesAlreadyOwned()
    {
        var store = NewStore();
        Assert.Equal("Your cart is empty.", store.Checkout("Asha", null).Error);

        AddToCart(store, "Asha", 2);
        Assert.Equal("Coupon not found.", store.Checkout("Asha", "NOPE").Error);
        Assert.Equal("This coupon has expired.", store.Checkout("Asha", "OLD5").Error);
        Assert.Empty(store.Orders);

        store.Enroll(2, "Asha"); // enrolled some other way after the course went into the cart
        Assert.Contains("already own", store.Checkout("Asha", null).Error);
    }

    // R16
    [Fact]
    public void R16_Checkout_SavesOrder_Enrolls_EmptiesCart_AndCountsCouponUse()
    {
        var store = NewStore();
        AddToCart(store, "Asha", 2, 3, 4);
        var (order, error) = store.Checkout("Asha", "welcome10");

        Assert.Null(error);
        Assert.Equal(1, order!.Id);
        Assert.Equal("Paid", order.Status);
        Assert.Equal(Today, order.CreatedAt);
        Assert.Equal("WELCOME10", order.CouponCode);
        Assert.Equal(76.50m, order.Total);
        Assert.Equal(3, order.Lines.Count);
        Assert.All(new[] { 2, 3, 4 }, id => Assert.True(store.IsEnrolled("Asha", id)));
        Assert.Empty(store.GetCart("Asha"));
        Assert.Equal(1, store.Coupons.First(c => c.Code == "WELCOME10").UsedCount);
    }

    // R17
    [Fact]
    public void R17_Orders_AreListedPerStudent()
    {
        var store = NewStore();
        AddToCart(store, "Asha", 2);
        store.Checkout("Asha", null);
        AddToCart(store, "Asha", 3);
        store.Checkout("Asha", null);
        AddToCart(store, "Ben", 4);
        store.Checkout("Ben", null);

        Assert.Equal(new[] { 2, 1 }, store.GetOrders("asha").Select(o => o.Id));
        Assert.Single(store.GetOrders("Ben"));
        Assert.Empty(store.GetOrders("Zoe"));
    }

    // R18
    [Fact]
    public void R18_Refund_IsRefused_WhenProgressIs40Percent()
    {
        var store = NewStore();
        AddToCart(store, "Asha", 4); // 5 lessons
        var orderId = store.Checkout("Asha", null).Order!.Id;
        CompleteLessons(store, "Asha", 4, 2); // 40%

        var result = store.RefundOrder(orderId);
        Assert.Null(result.Order);
        Assert.Contains("40%", result.Error);
        Assert.Contains("Data Analysis with SQL", result.Error);
        Assert.Equal("Paid", store.Orders[0].Status);
    }

    [Fact]
    public void R18_Refund_IsAllowedUpToSevenDays_ThenRefused()
    {
        var store = NewStore();
        AddToCart(store, "Asha", 2);
        var orderId = store.Checkout("Asha", null).Order!.Id;

        store.Clock = () => Today.AddDays(7);
        AddToCart(store, "Ben", 3);
        var late = store.Checkout("Ben", null).Order!.Id; // bought on day 7
        Assert.NotNull(store.RefundOrder(orderId).Order); // exactly 7 days is still fine

        store.Clock = () => Today.AddDays(14).AddHours(1);
        Assert.Contains("within 7 days", store.RefundOrder(late).Error);
    }

    [Fact]
    public void R18_Refund_Twice_IsAConflict_AndUnknownOrderIsNotFound()
    {
        var store = NewStore();
        AddToCart(store, "Asha", 2);
        var orderId = store.Checkout("Asha", null).Order!.Id;
        store.RefundOrder(orderId);

        Assert.Equal("This order was already refunded.", store.RefundOrder(orderId).Error);
        Assert.Equal("Order not found.", store.RefundOrder(999).Error);
    }

    // R19
    [Fact]
    public void R19_Refund_RemovesEnrollmentAndProgress_AndGivesBackTheCouponUse()
    {
        var store = NewStore();
        AddToCart(store, "Asha", 4, 3, 2);
        var orderId = store.Checkout("Asha", "WELCOME10").Order!.Id;
        CompleteLessons(store, "Asha", 4, 1); // 20%, under the limit

        var (order, error) = store.RefundOrder(orderId);

        Assert.Null(error);
        Assert.Equal("Refunded", order!.Status);
        Assert.Empty(store.Enrollments.Where(e => e.StudentName == "Asha"));
        Assert.Empty(store.Progress.Where(p => p.StudentName == "Asha"));
        Assert.Equal(0, store.Coupons.First(c => c.Code == "WELCOME10").UsedCount);

        // The student can use the coupon and buy the course again.
        AddToCart(store, "Asha", 4);
        Assert.NotNull(store.Checkout("Asha", "WELCOME10").Order);
    }

    [Fact]
    public void R19_EnrollmentIds_StayUnique_AfterARefund()
    {
        var store = NewStore();
        AddToCart(store, "Asha", 2);
        var orderId = store.Checkout("Asha", null).Order!.Id;
        store.Enroll(1, "Ben");
        store.RefundOrder(orderId);
        store.Enroll(5, "Cara");

        var ids = store.Enrollments.Select(e => e.Id).ToList();
        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    // R20
    [Fact]
    public void R20_InstructorRevenue_IsSeventyPercentOfNetSales()
    {
        var store = NewStore();
        AddToCart(store, "Asha", 2, 3, 4);               // Liam 40, Sofia 30, Ken 30 -> 34, 25.50, 25.50 after bundle
        store.Checkout("Asha", null);
        AddToCart(store, "Ben", 2);                       // Liam 40
        var benOrder = store.Checkout("Ben", null).Order!.Id;
        store.RefundOrder(benOrder);                      // Ben's 40 comes back

        var revenue = store.GetInstructorRevenue();
        var liam = revenue.First(r => r.Instructor == "Liam Carter");
        Assert.Equal(74m, liam.GrossSales);               // 34 + 40
        Assert.Equal(40m, liam.Refunds);
        Assert.Equal(23.80m, liam.InstructorShare);       // 70% of 34
        Assert.Equal(17.85m, revenue.First(r => r.Instructor == "Sofia Alvarez").InstructorShare); // 70% of 25.50
        Assert.Equal(0m, revenue.First(r => r.Instructor == "Maya Rao").GrossSales);
    }
}
