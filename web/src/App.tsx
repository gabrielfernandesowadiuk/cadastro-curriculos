import { BrowserRouter, Route, Routes } from 'react-router-dom'
import ListaPage from './pages/ListaPage'
import NovoPage from './pages/NovoPage'
import DetalhePage from './pages/DetalhePage'

export default function App() {
  return (
    <BrowserRouter>
      <main>
        <Routes>
          <Route path="/" element={<ListaPage />} />
          <Route path="/candidatos/novo" element={<NovoPage />} />
          <Route path="/candidatos/:id" element={<DetalhePage />} />
        </Routes>
      </main>
    </BrowserRouter>
  )
}