using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebIcecream.Models;

public partial class ProjectDak3Context : DbContext
{
    public ProjectDak3Context()
    {
    }

    public ProjectDak3Context(DbContextOptions<ProjectDak3Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<NewRecipe> NewRecipes { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-RQ8HM1R;Initial Catalog=ProjectDAK3;Persist Security Info=True;User ID=sa;Password=123;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Books__3DE0C22757103A6E");

            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ImageURL");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDF60D9AC6A7");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");
            entity.Property(e => e.FeedbackDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FeedbackText).HasColumnType("text");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__UserID__571DF1D5");
        });

        modelBuilder.Entity<NewRecipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PK__NewRecip__FDD988D0DE770321");

            entity.Property(e => e.RecipeId).HasColumnName("RecipeID");
            entity.Property(e => e.Flavor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ImageURL");
            entity.Property(e => e.Ingredients).HasColumnType("text");
            entity.Property(e => e.Procedure).HasColumnType("text");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SubmissionDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.NewRecipes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NewRecipe__UserI__5AEE82B9");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF330105C8");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.Cost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentOption)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Book).WithMany(p => p.Orders)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__BookID__534D60F1");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__UserID__52593CB8");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PK__Recipes__FDD988D03368BE4B");

            entity.Property(e => e.RecipeId).HasColumnName("RecipeID");
            entity.Property(e => e.Flavor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ImageURL");
            entity.Property(e => e.Ingredients).HasColumnType("text");
            entity.Property(e => e.Procedure).HasColumnType("text");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3AE15F4139");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACE9772B54");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053480CFE1FC").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Gender).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RegistrationDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserAcco__1788CCAC456B6405");

            entity.HasIndex(e => e.Username, "UC_Username").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("UserID");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.UserAccounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__UserAccou__RoleI__4F7CD00D");

            entity.HasOne(d => d.User).WithOne(p => p.UserAccount)
                .HasForeignKey<UserAccount>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserAccou__UserI__4E88ABD4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
