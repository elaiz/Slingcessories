import { useEffect, useState } from 'react'
import AccessoriesList from './components/AccessoriesList'
import LoginForm from './components/LoginForm'
import './App.css'
import { clearAuthToken, getAuthToken } from './auth'

function App() {
  const [authenticated, setAuthenticated] = useState<boolean>(false)

  useEffect(() => {
    setAuthenticated(!!getAuthToken())
  }, [])

  const handleLogout = () => {
    clearAuthToken()
    setAuthenticated(false)
  }

  if (!authenticated) {
    return (
      <div className="App">
        <LoginForm onLoggedIn={() => setAuthenticated(true)} />
      </div>
    )
  }

  return (
    <div className="App">
      <header className="app-header">
        <button type="button" className="logout-button" onClick={handleLogout}>
          Log out
        </button>
      </header>
      <AccessoriesList />
    </div>
  )
}

export default App
