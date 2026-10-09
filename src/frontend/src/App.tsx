import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { MainLayout } from './layouts/MainLayout';
import Home from './pages/Home';
import ListView from './pages/ListView';
import CardView from './pages/CardView';
import Ask from './pages/Ask';
import './App.css';

const App = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<MainLayout />}>
          <Route index element={<Home />} />
          <Route path="list" element={<ListView />} />
          <Route path="cards" element={<CardView />} />
          <Route path="ask" element={<Ask />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
};

export default App;
