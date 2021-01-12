#r "../_lib/Fornax.Core.dll"
#r "../_lib/Markdig.dll"
#load "../globals.fsx"

open Markdig
open Globals

type DataSciencePackage = {
    PackageName : string
    PackageLogoLink : string
    PackageNugetLink : string
    PackageGithubLink : string
    PackageDocumentationLink : string
    PackageDescription: string
    PackageMore: string option
    PackagePostsLink: string option
} with
    static member create pName pLogo pNuget pGithub pDocs pDesc pMore pPosts = 
        {
            PackageName = pName
            PackageLogoLink = pLogo
            PackageNugetLink = pNuget
            PackageGithubLink = pGithub
            PackageDocumentationLink = pDocs
            PackageDescription = pDesc
            PackageMore = pMore
            PackagePostsLink = pPosts
        }

let contentDir = "content/packages"

let loadFile (packageMarkdownPath:string) =

    let text = System.IO.File.ReadAllText packageMarkdownPath

    let config = MarkdownProcessing.getFrontMatter text
    let content = MarkdownProcessing.getMarkdownContent text

    let name =  config |> Map.find "package-name"
    let logo = config |> Map.find "package-logo"
    let nuget = config |> Map.find "package-nuget-link"
    let github = config |> Map.find "package-github-link"
    let docs = config |> Map.find "package-documentation-link"
    let desc = config |> Map.find "package-description"
    let posts = config |> Map.tryFind "package-posts-link"

    DataSciencePackage.create name logo nuget github docs desc (if content = "" then None else Some content) posts


let loader (projectRoot: string) (siteContent: SiteContents) =
    let cardsPath = System.IO.Path.Combine(projectRoot, contentDir)
    System.IO.Directory.GetFiles cardsPath
    |> Array.filter Predicates.isMarkdownFile
    |> Array.iter (fun path ->
        try 
            printfn "[LOADER]: Adding package at %s" path
            siteContent.Add (loadFile path)
        with _ -> 
            siteContent.AddError {Path = path; Message = (sprintf "Uable to load card %s" path); Phase = Loading}
    )
    siteContent