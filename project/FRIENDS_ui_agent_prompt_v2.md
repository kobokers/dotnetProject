# FRIENDS Social Network — UI Agent Prompt v2
> Paste this entire prompt into OpenCode when starting UI work.
> Updated: Coral + OLED Dark Mode, 3-column layout based on approved design.

---

## Context

We are building a social network web app called **F.R.I.E.N.D.S** using:
- ASP.NET Core MVC (C#)
- Entity Framework Core with SQL Server LocalDB
- Razor Views (`.cshtml`)
- ASP.NET Core Identity for auth

The project has **4 modules**: Profile, Posts, Friends, Comments.  
All views must use a shared `_Layout.cshtml`.

---

## UI Goal — Approved Design + Coral OLED Dark Mode

The layout is a **3-column design**:
- Left: fixed sidebar navigation (like the reference design)
- Center: main content feed (posts, forms, etc.)
- Right: suggestions panel (suggested friends, trending)

### Color Palette — Coral OLED Dark

```css
:root {
  /* OLED blacks */
  --bg-primary: #000000;        /* true OLED black, main background */
  --bg-secondary: #0a0a0a;      /* slightly lifted, sidebar bg */
  --bg-elevated: #111111;       /* cards, inputs */
  --bg-hover: #1a1a1a;          /* hover state */
  --border-color: #1f1f1f;      /* subtle borders */

  /* Coral accent */
  --accent: #ff6b6b;            /* primary coral */
  --accent-hover: #ff4f4f;      /* darker coral on hover */
  --accent-soft: #ff6b6b22;     /* coral tint for backgrounds */
  --accent-text: #ff8e8e;       /* lighter coral for text links */

  /* Text */
  --text-primary: #ffffff;
  --text-secondary: #888888;
  --text-muted: #444444;

  /* Status */
  --danger: #ff4f4f;
  --success: #2ecc71;
  --online: #2ecc71;

  /* Sizing */
  --sidebar-width: 245px;
  --right-panel-width: 300px;
  --feed-max-width: 630px;
  --radius-sm: 4px;
  --radius-md: 12px;
  --radius-lg: 16px;
  --radius-full: 9999px;
}
```

Font: `Inter` from Google Fonts — import in `_Layout.cshtml` head.

---

## Layout Structure — `_Layout.cshtml`

```
┌─────────────┬──────────────────────┬─────────────────┐
│  LEFT       │   CENTER FEED        │  RIGHT PANEL    │
│  SIDEBAR    │   (max 630px)        │  (300px)        │
│  (245px)    │                      │                 │
│  fixed      │   scrollable         │  sticky         │
└─────────────┴──────────────────────┴─────────────────┘
```

### Left Sidebar
- Background: `var(--bg-secondary)`
- Border-right: `1px solid var(--border-color)`
- Fixed position, full height
- Top: App logo "F.R.I.E.N.D.S" — coral color, bold
- Bottom: current user avatar + username + `...` menu

Nav items (icon + label):
```
🏠  Home          → /Post/Index
🔍  Search        → /Account/Search
🧭  Explore       → /Post/Index (same feed for now)
🔔  Notifications → /Friend/Requests  (show red badge with count)
✉️  Messages      → # (placeholder)
👤  Profile       → /Account/Profile
➕  Create Post   → /Post/Create   ← coral filled button, full width
```

Nav item style:
- Padding: 12px 16px
- Border-radius: `var(--radius-md)`
- Hover: `background: var(--bg-hover)`
- Active: `background: var(--accent-soft)`, text coral
- Icon size: 20px, margin-right: 12px
- "Create Post" button: `background: var(--accent)`, white text, full width, rounded

### Center Feed
- Margin-left: `var(--sidebar-width)`
- Margin-right: `var(--right-panel-width)`
- Padding: 24px 32px
- Max-width: `var(--feed-max-width)` centered

### Right Panel
- Fixed right, width `var(--right-panel-width)`
- Background: `var(--bg-secondary)`
- Border-left: `1px solid var(--border-color)`
- Padding: 24px 16px
- Contains: Search bar, filter tabs, Suggested for you, Trending Topics

---

## CSS — Key Component Styles

### Post Card
```css
.post-card {
  background: var(--bg-elevated);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-lg);
  margin-bottom: 16px;
  padding: 16px;
}
```

### Input / Textarea
```css
input, textarea {
  background: var(--bg-elevated);
  border: 1px solid var(--border-color);
  color: var(--text-primary);
  border-radius: var(--radius-sm);
  padding: 10px 12px;
}
input:focus, textarea:focus {
  border-color: var(--accent);
  outline: none;
  box-shadow: 0 0 0 2px var(--accent-soft);
}
```

### Buttons
```css
.btn-primary {
  background: var(--accent);
  color: white;
  border: none;
  border-radius: var(--radius-full);
  padding: 8px 20px;
  font-weight: 600;
}
.btn-primary:hover { background: var(--accent-hover); }

.btn-outline {
  background: transparent;
  color: var(--text-primary);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-full);
  padding: 8px 20px;
}
.btn-outline:hover { border-color: var(--accent); color: var(--accent); }
```

### Avatar
```css
.avatar {
  border-radius: 50%;
  object-fit: cover;
  border: 2px solid var(--border-color);
}
.avatar-sm  { width: 32px;  height: 32px; }
.avatar-md  { width: 48px;  height: 48px; }
.avatar-lg  { width: 80px;  height: 80px; }
.avatar-xl  { width: 150px; height: 150px; }
/* Coral ring for active/online */
.avatar-active { border-color: var(--accent); }
```

### Story Row (top of feed)
- Horizontal scroll row of circle avatars with username below
- Each story circle: coral gradient border ring (`border: 2px solid var(--accent)`)
- "Add Story" first item: dashed border, `+` icon

---

## Views to Build

### `/Account/Login`
- Full page centered card (max 400px)
- True black background
- Logo at top in coral
- Email + Password dark inputs
- "Log in" coral button full width
- "Don't have an account? Sign up" link below

### `/Account/Register`
- Same style as login
- Fields: Username, Email, Password, Confirm Password
- "Sign up" coral button

### `/Post/Index` — Feed (Home)
Top of feed: **Story row** — horizontal scroll of friend avatars with coral ring

Each post card:
```
┌─────────────────────────────────────────┐
│ [avatar] Username        timestamp   ⋮  │
│                                         │
│ Post content text here...  See More     │
│                                         │
│ [Post image full width if any]          │
│                                         │
│ ❤️ 2.8K  ●●○  948 Comments             │
│─────────────────────────────────────────│
│ 🤍 Like    💬 Comment    ↗ Share        │
└─────────────────────────────────────────┘
```
- Like button turns coral when liked
- "See More" expands text (JS toggle)
- Multi-image posts: show 2-image grid below main image

### `/Post/Create`
- Centered card max 500px
- Textarea (min-height 120px) for content
- Image upload input
- "Share" coral button

### `/Post/Details`
- Full post card at top
- Comments section:
  - Each comment: avatar + username (coral, bold) + text + delete (own only, small red)
- Add comment: input row at bottom with "Post" coral text button

### `/Account/Profile`
```
┌──────────────────────────────────────────┐
│  [avatar-xl]   Username  (bold, large)   │
│                Bio text                  │
│                📍 Location               │
│                [Edit Profile] [btn]      │
│                                          │
│  Posts: 12    Friends: 48               │
└──────────────────────────────────────────┘
3-column image grid below (post thumbnails)
```

### `/Account/Edit`
- Form: Display Name, Bio, Location, Profile Photo upload
- Save coral button

### `/Friend/Index`
- Grid of friend cards (3 per row)
- Each: avatar + username + "Unfriend" outline button
- Empty state with coral icon

### `/Friend/Requests`
Two tabs: "Received" | "Sent" (coral underline for active tab)
- Each request card: avatar + username + Accept (coral) / Reject (outline) buttons

### `/Account/Search`
- Search bar full width at top (dark, coral focus ring)
- Results: user rows with avatar + username + Add Friend / Friends button

---

## Right Panel Content

### Search Bar
```html
<input type="text" placeholder="Search..." class="search-input" />
```
Filter tabs: `All | Popular | Account` — coral underline on active

### Suggested for You
List of 3–5 suggested users:
- Avatar (32px) + username (bold) + subtitle ("Followed by X") + Follow button (coral text)

### Trending Topics
Hashtag pills:
```css
.hashtag-pill {
  background: var(--bg-hover);
  border-radius: var(--radius-full);
  padding: 4px 12px;
  font-size: 13px;
  color: var(--accent-text);
  margin: 4px;
  display: inline-block;
}
```

---

## Razor-Specific Rules

- Use `@model` strongly typed views — never `ViewBag` for lists
- Use `asp-controller`, `asp-action` tag helpers for all links and forms
- Use `asp-for` on all form inputs
- Use `@Html.ValidationMessageFor` on form fields
- Comments section = partial view `_Comments.cshtml`
- Profile photo served from `wwwroot/uploads/`
- Story row = partial view `_Stories.cshtml`
- Right panel = partial view `_RightPanel.cshtml`
- `@if (User.Identity.IsAuthenticated)` to show/hide auth elements
- `@if (Model.UserId == currentUserId)` for own content edit/delete

---

## Folder Structure

```
/Views
  /Account   — Login, Register, Profile, Edit, Search
  /Post      — Index, Create, Edit, Details, Delete
  /Friend    — Index, Requests
  /Shared    — _Layout.cshtml, _Comments.cshtml,
               _Stories.cshtml, _RightPanel.cshtml
/wwwroot
  /css       — site.css
  /uploads   — profile photos, post images
```

---

## Icons — Bootstrap Icons ONLY

**Do NOT use emojis anywhere in the UI.** Use Bootstrap Icons (`bi` classes) for all icons.

Add this to `_Layout.cshtml` head:
```html
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
```

Icon mappings to use:
```
Home         → <i class="bi bi-house-fill"></i>
Search       → <i class="bi bi-search"></i>
Explore      → <i class="bi bi-compass"></i>
Notifications→ <i class="bi bi-bell"></i>
Messages     → <i class="bi bi-chat"></i>
Profile      → <i class="bi bi-person-circle"></i>
Create Post  → <i class="bi bi-plus-square"></i>
Like         → <i class="bi bi-heart"></i>  /  <i class="bi bi-heart-fill"></i> (liked)
Comment      → <i class="bi bi-chat-dots"></i>
Share        → <i class="bi bi-send"></i>
Delete       → <i class="bi bi-trash"></i>
Edit         → <i class="bi bi-pencil"></i>
Settings     → <i class="bi bi-three-dots"></i>
Follow       → <i class="bi bi-person-plus"></i>
Unfriend     → <i class="bi bi-person-dash"></i>
Location     → <i class="bi bi-geo-alt"></i>
Add Story    → <i class="bi bi-plus-circle"></i>
Online badge → <i class="bi bi-circle-fill"></i> (green, small)
```

For the app logo/brand mark — use a text logo styled in CSS, or an SVG file placed in `wwwroot/images/logo.svg`. Do not use an emoji as a logo.

---

## Do NOT Do

- Do not use default Bootstrap navbar or cards — write custom CSS
- Do not use light/white backgrounds anywhere — OLED black only
- Do not use blue as accent — coral (`#ff6b6b`) is the only accent color
- Do not use `ViewBag` for passing lists
- Do not use inline styles — use CSS variables
- Do not hardcode routes — use tag helpers
- Do not add the right panel inside pages that don't need it (Login, Register)
- Do not use jQuery unless needed for a specific interaction
- **Do not use emojis** — use Bootstrap Icons (`bi` classes) only
- Do not use random PNG images as placeholders — use CSS initials avatar as fallback
