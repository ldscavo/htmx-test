open Saturn
open Giraffe.ViewEngine
open Giraffe.ViewEngine.Htmx
open Giraffe.Htmx
open Microsoft.AspNetCore.Http

let template content =
    html [] [
        head [] [
            meta [_charset "utf-8"]
            meta [_name "viewport"; _content "width=device-width, initial-scale=1"]
            title [] [str "HTMX Test"]
            script [_src "https://unpkg.com/htmx.org@1.7.0"] []
        ]
        body [] [
            div [_id "body"] [
                h1 [] [str "Title"]
                div [_id "content"; _hxBoost; _hxTarget "#content"] [
                    content
                ]
            ]
        ]
    ]

let render (ctx: HttpContext) page =
    match ctx.Request.IsHtmx && not ctx.Request.IsHtmxRefresh with
    | true -> page
    | false -> page |> template

let home ctx =
    task {
        return
            div [] [
                div [] [a [_href "/about"] [str "About"]]
                div [] [
                    form [_action "/clicked"; _method "POST"] [
                        button [_type "submit"] [str "Click Me!"]
                    ]
                ]
            ]
            |> (render ctx)
    }    

let about ctx =
    task {
        return
            div [] [
                h3 [] [str "About"]
                p [] [
                    str """
                        Lorem ipsum dolor sit amet, consectetur adipiscing elit,
                        sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
                        Odio eu feugiat pretium nibh ipsum consequat nisl vel.
                        Tincidunt vitae semper quis lectus nulla at volutpat.
                        Tincidunt dui ut ornare lectus sit amet.
                        Pellentesque adipiscing commodo elit at imperdiet dui.
                        Lobortis mattis aliquam faucibus purus in.
                        Etiam erat velit scelerisque in dictum non consectetur a.
                        Ac feugiat sed lectus vestibulum mattis ullamcorper velit sed.
                        Egestas tellus rutrum tellus pellentesque eu tincidunt tortor aliquam.
                        Et malesuada fames ac turpis egestas sed
                    """
                ]
            ]
            |> (render ctx)
    }

let clicked ctx =
    task {
        return
            div [_style "background:#bba169; color:#fff;"] [
                p [] [str "You clicked the button!"]
            ]
            |> (render ctx)
    }

let indexController =
    controller {
        index home
    }

let aboutController =
    controller {
        index about
    }

let clickController =
    controller {
        index clicked
        create clicked
    }

let routes =
    router {
        forward "" indexController
        forward "/about" aboutController
        forward "/clicked" clickController
    }

let app =
    application {
        use_router routes
    }

run app