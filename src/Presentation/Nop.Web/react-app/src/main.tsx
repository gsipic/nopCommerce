import {lazy, StrictMode} from 'react'
import {hydrateRoot} from 'react-dom/client'
import './index.css'
import {DropdownProps} from './App.tsx'

const App = lazy(() => import("./App"));

// @ts-ignore
var foo = window.carSearchData as [DropdownProps];

hydrateRoot(document.getElementById('root')!,
    <StrictMode>
        <App dropdowns={foo}/>
    </StrictMode>
)
