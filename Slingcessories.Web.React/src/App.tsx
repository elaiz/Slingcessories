import { useEffect, useState } from 'react'
import AccessoriesList from './components/AccessoriesList'
import LoginForm from './components/LoginForm'
import NavMenu, { NavView } from './components/NavMenu'
import SlingshotsList from './components/SlingshotsList'
import CategoriesPage from './components/CategoriesPage'
import SettingsPage from './components/SettingsPage'
import HomePage from './components/HomePage'
import RegisterForm from './components/RegisterForm'
import './App.css'
import { clearAuthToken, getAuthToken } from './auth'
import { UserInfo } from './services/api'

function App() {
  const [authenticated, setAuthenticated] = useState<boolean>(false)
  const [user, setUser] = useState<UserInfo | null>(null)
  const [currentView, setCurrentView] = useState<NavView>('home')
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const [showRegister, setShowRegister] = useState(false)

  useEffect(() => {
    setAuthenticated(!!getAuthToken())
  }, [])

  const handleLoggedIn = (userInfo: UserInfo) => {
    setAuthenticated(true)
    setUser(userInfo)
    setCurrentView('home')
  }

  const handleLogout = () => {
    clearAuthToken()
    setAuthenticated(false)
    setUser(null)
    setCurrentView('home')
    setShowRegister(false)
  }

  const navigate = (view: NavView) => {
    setCurrentView(view)
    setSidebarOpen(false)
  }

  if (!authenticated) {
    if (showRegister) {
      return (
        <div className="App">
          <RegisterForm
            onRegistered={handleLoggedIn}
            onBackToLogin={() => setShowRegister(false)}
          />
        </div>
      )
    }
    return (
      <div className="App">
        <LoginForm onLoggedIn={handleLoggedIn} onRegister={() => setShowRegister(true)} />
      </div>
    )
  }

  const renderContent = () => {
    switch (currentView) {
      case 'home': return <HomePage onNavigate={navigate} />
      case 'accessories': return <AccessoriesList filterWishlist={false} />
      case 'wishlist': return <AccessoriesList filterWishlist={true} />
      case 'slingshots': return <SlingshotsList />
      case 'settings': return <SettingsPage onNavigate={navigate} />
      case 'categories': return <CategoriesPage onBack={() => navigate('settings')} />
      default: return <HomePage onNavigate={navigate} />
    }
  }

  return (
    <div className="page">
      <div className={`sidebar${sidebarOpen ? ' sidebar-open' : ''}`}>
        <NavMenu currentView={currentView} onNavigate={navigate} />
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
            <button className="btn-link" onClick={() => navigate('settings')}>Settings</button>
          </div>
        </div>

        <article className="content">
          {renderContent()}
        </article>
      </main>
    </div>
  )
}

export default App
