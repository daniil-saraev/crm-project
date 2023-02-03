import { Routes, Route } from 'react-router-dom'
import Navbar from './components/navbar'
import Contacts from './pages/contacts'
import Home from './pages/home'
import Projects from './pages/projects'
import Services from './pages/services'
import { ChakraProvider } from '@chakra-ui/react'
import theme from './theme'

const App = () => {
  return (
      <ChakraProvider theme={theme}>
        <div className="app">
          <Navbar/>
          <main className="content">
            <Routes>
              <Route path="/" element={<Home/>}/>
              <Route path="/services" element={<Services/>}/>
              <Route path="/projects" element={<Projects/>}/>
              <Route path="/contacts" element={<Contacts/>}/>
            </Routes>
          </main>
        </div>
      </ChakraProvider>
  )
}

export default App
