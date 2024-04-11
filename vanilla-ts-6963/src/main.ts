import "./style.css"
import { listProviders } from "/workspaces/main/vanilla-ts-6963/providers.ts"

document.querySelector<HTMLDivElement>('#app')!.innerHTML = `
    <div>
        <div id="providerButtons"></div>
    </div>
`

listProviders(document.querySelector<HTMLDivElement>("#providerButtons")!)