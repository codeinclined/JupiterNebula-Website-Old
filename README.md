[![Build Status](https://jupiternebula.visualstudio.com/JupiterNebula-Website/_apis/build/status/JupiterNebula.JupiterNebula-Website?branchName=master)](https://jupiternebula.visualstudio.com/JupiterNebula-Website/_build/latest?definitionId=1?branchName=master)
[![Netlify Status](https://api.netlify.com/api/v1/badges/5ae84d57-4dce-423b-96f0-7010a1e6699b/deploy-status)](https://app.netlify.com/sites/serene-stonebraker-113a52/deploys)

[Link to Production JupiterNebula Website](https://jupiternebula.com)

# JupiterNebula-Website
This repository holds the input and theme sources for the personal blog of Josh Taylor, [jupiternebula.com](https://jupiternebula.com).

## Tech Stack

### Static Generation
The JupiterNebula website is statically generated using [Wyam](https://wyam.io), an open
source site generator. Wyam leverages [ASP.NET Core](https://asp.net) along with a suite
of modules to render static content with access to all of the power of .NET Core and NuGet.
I have found it to be quite flexible, powerful, and familiar (since I write full stack
C# applications for a living). I wanted something that allows me to take advantage of my
experience with .NET while maintaining the economy and simplicity of static generation.
Wyam fits the bill perfectly.

I am currently using the Blog recipe with a custom "theme" that I've been putting together over
time. I put "theme" in quotes so I can clarify that I am using the word in terms of Wyam's
Blog recipe and not in terms of CSS styling (to be discussed in the next section).

### Styling
It might be cliche for an ASP.NET Core developer to choose
[Bootstrap](https://getbootstrap.com), but it definitely makes things easier (especially
for something I doubt too many people will even be looking at). I am using the
[Darkly](https://bootswatch.com/darkly) Bootstrap theme from the great
[Bootswatch](https://bootswatch.com) website. I have found Bootswatch to be an invaluable
resource for quickly putting together attractive sites when you don't have the time or means
to go all out on creating custom styling from scratch.

### Icons
I am using [Font Awesome](https://fontawesome.com) for my social icons. I find their icons
to be of excellent quality. It is very simple to find and add their icons to your site,
a huge number (1,500) of them are free, and they provide a CDN link for their CSS. I have
used the Material Design icons in the past, but I am much happier with the aesthetics and
variety that Font Awesome provides.

## Responsive Design
I have made an effort to take advantage of the responsive CSS classes provided
by Bootstrap to make the site as accessible as possible on a variety of
screen sizes. Feel free to reach out to me if something breaks or looks ugly
on your device.

## Revision Control & CI/CD
All source files are hosted on [GitHub](https://github.com). Pushes and pull requests to the
master branch trigger a build pipeline on [Azure DevOps](https://dev.azure.com) which simply
builds the site using the Wyam CLI tool, packages the site, and publishes it to
[Netlify](https://netlify.com) using curl. Netlify is a fantastic hosting provider with
free hosting available for personal sites. They offer CI/CD services; however, they do not
currently provide .NET Core on their build agents.

Refer to [azure-pipelines.yml](/azure-pipelines.yml) for details on how I
build, package, and deploy my site to Netlify using GitHub and Azure Pipelines.
