# SPEC-001 — Project page indexing

Status: Proposed
Owner: naindev
Scope: `site/`, `scripts/`, `.github/workflows/ci.yml`

## Problem

`https://template-dotnet.naindev.com/` is a public page, but it declares no
canonical URL and publishes no sitemap. Google Search Console lists the
`http://` variant as "Duplicate: no canonical selected" and the page cannot be
submitted to Google or Bing through a sitemap. It also lacks the Open Graph and
structured data that the other NainDev project pages carry.

## Requirements

Notation: EARS.

- **REQ-IDX-001** — The page shall declare
  `https://template-dotnet.naindev.com/` as its canonical URL.
- **REQ-IDX-002** — The page shall declare Open Graph and Twitter card metadata
  (`og:type`, `og:title`, `og:description`, `og:url`, `og:image`,
  `twitter:card`).
- **REQ-IDX-003** — The page shall embed JSON-LD of type `SoftwareSourceCode`
  that points to the page URL and to the GitHub repository.
- **REQ-IDX-004** — The page shall not carry a `noindex` directive.
- **REQ-IDX-005** — The site shall publish `/sitemap.xml` listing only the page
  URL.

## Out of scope

- Content, layout and copy of the page.
- Submitting the sitemap to Search Console and Bing Webmaster (owner step, done
  after the deploy).
