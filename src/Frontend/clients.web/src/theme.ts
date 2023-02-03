import { extendTheme } from "@chakra-ui/react"

const theme = extendTheme({
  colors: {
    black: '#12130F',
    white: '#e9ecf5',
    grey: '#2b2d42',
    red: '#DB162F',
    green: '#55917F',
    blue: '#455A93'
  },
  styles: {
    global: {
      body: {
        fontFamily: 'Roboto Mono'
      }
    }
}})

export default theme;