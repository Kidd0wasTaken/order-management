import { createTheme } from '@mui/material/styles';

const theme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#1e3a5f',
      light: '#2d5a8e',
      dark: '#122640',
    },
    secondary: {
      main: '#0d9488',
    },
    background: {
      default: '#f4f6f9',
      paper: '#ffffff',
    },
  },
  typography: {
    fontFamily: '"DM Sans", "Segoe UI", Roboto, Helvetica, Arial, sans-serif',
    h5: {
      fontWeight: 700,
      letterSpacing: '-0.02em',
    },
  },
  shape: {
    borderRadius: 10,
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          textTransform: 'none',
          fontWeight: 600,
        },
      },
    },
    MuiTableCell: {
      styleOverrides: {
        head: {
          fontWeight: 600,
          backgroundColor: '#f8fafc',
        },
      },
    },
  },
});

export default theme;
