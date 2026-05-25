import { useEffect, useState } from 'react'
import AccessoriesList from './components/AccessoriesList'
import LoginForm from './components/LoginForm'
import NavMenu, { NavView } from './components/NavMenu'
import './App.css'
import { clearAuthToken, getAuthToken } from './auth'
import { UserInfo } from './services/api'

function App() {
  const [authenticated, setAuthenticated] = useState<boolean>(false)
  const [user, setUser] = useState<UserInfo | null>(null)
  const [currentView, setCurrentView] = useState<NavView>('all')
  const [sidebarOpen, setSidebarOpen] = useState(false)

  useEffect(() => {
    setAuthenticated(!!getAuthToken())
  }, [])

  const handleLoggedIn = (userInfo: UserInfo) => {
    setAuthenticated(true)
    setUser(userInfo)
  }

  const handleLogout = () => {
    clearAuthToken()
    setAuthenticated(false)
    setUser(null)
    setCurrentView('all')
  }

  const filterWishlist =
    currentView === 'wishlist' ? true :
    currentView === 'accessories' ? false :
    undefined

  if (!authenticated) {
    return (
      <div className="App">
        <LoginForm onLoggedIn={handleLoggedIn} />
      </div>
    )
  }

  return (
    <div className="page">
      <div className={`sidebar${sidebarOpen ? ' sidebar-open' : ''}`}>
        <NavMenu
          currentView={currentView}
          onNavigate={(v) => { setCurrentView(v); setSidebarOpen(false) }}
        />
      </div>

      {sidebarOpen && (
        <div className="sidebar-overlay" onClick={() => setSidebarOpen(false)}></div>
      )}

      <main>
        <div className="top-row">
          <div className="top-row-left">
            <button
              className="hamburger-btn"
              onClick={() => setSidebarOpen(!sidebarOpen)}
              aria-label="Toggle navigation"
            >
              <span className="navbar-toggler-icon"></span>
            </button>
            {user && (
              <span className="user-info">
                <span className="user-icon" aria-hidden="true">👤</span>
                <strong>{user.firstName} {user.lastName}</strong>
              </span>
            )}
            <button className="btn-link" onClick={handleLogout}>Log out</button>
          </div>
          <div className="top-row-right">
            <button className="btn-link">Settings</button>
          </div>
        </div>

        <article className="content">
          <AccessoriesList filterWishlist={filterWishlist} />
        </article>
      </main>
    </div>
  )
}

export default App
