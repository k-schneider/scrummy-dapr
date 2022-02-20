module.exports = {
  content: [
    "./Components/**/*.razor",
    "./Pages/**/*.razor",
    "./wwwroot/**/*.{html,js}",
  ],
  theme: {
    extend: {
      backgroundImage: {
        'hero-cas': "url(\"data:image/svg+xml,%3Csvg width='40' height='40' viewBox='0 0 40 40' xmlns='http://www.w3.org/2000/svg'%3E%3Cpath d='M0 0h20v20H0V0zm10 17a7 7 0 1 0 0-14 7 7 0 0 0 0 14zm20 0a7 7 0 1 0 0-14 7 7 0 0 0 0 14zM10 37a7 7 0 1 0 0-14 7 7 0 0 0 0 14zm10-17h20v20H20V20zm10 17a7 7 0 1 0 0-14 7 7 0 0 0 0 14z' fill='%237d58ba' fill-opacity='0.04' fill-rule='evenodd'/%3E%3C/svg%3E\")"
      },
      fontFamily: {
        'play': ['Play', 'sans-serif']
      }
    }
  },
  plugins: [],
}
